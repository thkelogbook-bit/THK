using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Tích hợp với Open Notebook API — upload file tài liệu Elogbook
    /// sang Open Notebook để làm nguồn cho AI research assistant.
    ///
    /// Luồng upload KnowledgeBase:
    ///   1. EnsureKnowledgeBaseNotebookAsync()
    ///      — GET /api/notebooks → tìm notebook tên "KnowledgeBase"
    ///      — Nếu chưa tồn tại: POST /api/notebooks { name: "KnowledgeBase" }
    ///      — Cache ID trong bộ nhớ (TTL 1 giờ)
    ///   2. POST /api/sources (multipart) → tạo source, nhận source_id
    ///   3. POST /api/notebooks/{notebookId}/sources/{sourceId} → link
    ///
    /// Xác thực: Authorization: Bearer {OPEN_NOTEBOOK_PASSWORD}
    /// </summary>
    public class OpenNotebookService : IOpenNotebookService
    {
        private const string SOURCES_ENDPOINT         = "/api/sources";
        private const string HEALTH_ENDPOINT          = "/health";
        private const string NOTEBOOKS_ENDPOINT       = "/api/notebooks";
        private const string LINK_SOURCE_ENDPOINT     = "/api/notebooks/{0}/sources/{1}";
        private const string ASK_SIMPLE_ENDPOINT      = "/api/search/ask/simple";
        private const string MODELS_ENDPOINT          = "/api/models";
        private const string MODELS_DEFAULTS_ENDPOINT = "/api/models/defaults";
        private const string KNOWLEDGE_BASE_NOTEBOOK  = "KnowledgeBase";

        // Cache notebook ID để tránh gọi GET /api/notebooks mỗi lần upload
        private static string _cachedNotebookId;
        private static DateTime _cacheExpiry = DateTime.MinValue;
        private static readonly TimeSpan _cacheTtl = TimeSpan.FromHours(1);
        private static readonly SemaphoreSlim _notebookLock = new SemaphoreSlim(1, 1);

        // Cache default language model ID
        private static string _cachedDefaultModelId;
        private static DateTime _modelCacheExpiry = DateTime.MinValue;
        private static readonly SemaphoreSlim _modelLock = new SemaphoreSlim(1, 1);

        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly ElogbookUnitOfWork _unitOfWork;

        public OpenNotebookService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =====================================================================
        // Public — IOpenNotebookService
        // =====================================================================

        /// <inheritdoc/>
        public async Task<ResponseModel<List<OpenNotebookUploadResult>>> UploadFilesAsync(
            IEnumerable<OpenNotebookUploadItem> files)
        {
            var response = new ResponseModel<List<OpenNotebookUploadResult>>();
            response.Data = new List<OpenNotebookUploadResult>();

            var baseUrl = GetBaseUrl();
            if (baseUrl == null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("OPEN_NOTEBOOK_BASE_URL chưa được cấu hình.");
                return response;
            }

            foreach (var file in files)
            {
                var result = new OpenNotebookUploadResult { FileName = file.FileName };
                try
                {
                    result = await UploadSingleFileAsync(baseUrl, file);
                }
                catch (Exception ex)
                {
                    result.IsError = true;
                    result.ErrorMessage = ex.Message;
                    LogCustom.LogMessage(
                        TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                        string.Format("UploadFilesAsync — file: {0}{1}{2}", file.FileName, Environment.NewLine, ex));
                }
                response.Data.Add(result);
            }

            return response;
        }

        /// <inheritdoc/>
        public async Task<string> UploadKnowledgeBaseFileAndGetIdAsync(string filePath, string fileName, string title)
        {
            if (string.IsNullOrWhiteSpace(AppSettings.OPEN_NOTEBOOK_BASE_URL))
                return null;

            if (!File.Exists(filePath))
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    string.Format("UploadKnowledgeBaseFileAndGetIdAsync: File không tồn tại — {0}", filePath));
                return null;
            }

            try
            {
                var notebookId = await EnsureKnowledgeBaseNotebookAsync();
                if (string.IsNullOrWhiteSpace(notebookId)) return null;

                var mimeType = ResolveMimeType(fileName);
                using (var stream = File.OpenRead(filePath))
                {
                    var uploadItem = new OpenNotebookUploadItem
                    {
                        FileName   = fileName,
                        MimeType   = mimeType,
                        Content    = stream,
                        Title      = title,
                        NotebookId = notebookId,
                    };

                    var baseUrl = GetBaseUrl();
                    var result  = await UploadSingleFileAsync(baseUrl, uploadItem);

                    if (!result.IsError && !string.IsNullOrWhiteSpace(result.SourceId))
                    {
                        LogCustom.LogMessage(TrackingLogLevel.INFO, Category.OpenNotebook.ToString(),
                            string.Format("UploadKnowledgeBaseFileAndGetIdAsync: '{0}' → sourceId={1}",
                                fileName, result.SourceId));
                        return result.SourceId;
                    }

                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        string.Format("UploadKnowledgeBaseFileAndGetIdAsync: lỗi '{0}': {1}",
                            fileName, result.ErrorMessage));
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("UploadKnowledgeBaseFileAndGetIdAsync — file: {0}{1}{2}", fileName, Environment.NewLine, ex));
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task UploadKnowledgeBaseFileAsync(string filePath, string fileName, string title)
        {
            // Bỏ qua ngay nếu chưa cấu hình — không throw để không ảnh hưởng luồng chính
            if (string.IsNullOrWhiteSpace(AppSettings.OPEN_NOTEBOOK_BASE_URL))
            {
                LogCustom.LogMessage(TrackingLogLevel.DEBUG, Category.OpenNotebook.ToString(),
                    "UploadKnowledgeBaseFileAsync: OPEN_NOTEBOOK_BASE_URL chưa cấu hình — bỏ qua.");
                return;
            }

            if (!File.Exists(filePath))
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    string.Format("UploadKnowledgeBaseFileAsync: File không tồn tại — {0}", filePath));
                return;
            }

            try
            {
                // Bước 1: Đảm bảo notebook "KnowledgeBase" tồn tại
                var notebookId = await EnsureKnowledgeBaseNotebookAsync();
                if (string.IsNullOrWhiteSpace(notebookId))
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        "UploadKnowledgeBaseFileAsync: Không thể lấy notebook KnowledgeBase — bỏ qua.");
                    return;
                }

                // Bước 2: Đọc file và upload
                var mimeType = ResolveMimeType(fileName);
                using (var stream = File.OpenRead(filePath))
                {
                    var uploadItem = new OpenNotebookUploadItem
                    {
                        FileName  = fileName,
                        MimeType  = mimeType,
                        Content   = stream,
                        Title     = title,
                        NotebookId = notebookId,
                    };

                    var baseUrl = GetBaseUrl();
                    var result  = await UploadSingleFileAsync(baseUrl, uploadItem);

                    if (result.IsError)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                            string.Format("UploadKnowledgeBaseFileAsync — lỗi upload '{0}': {1}",
                                fileName, result.ErrorMessage));
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.INFO, Category.OpenNotebook.ToString(),
                            string.Format("UploadKnowledgeBaseFileAsync — thành công '{0}', sourceId={1}, status={2}",
                                fileName, result.SourceId, result.Status));
                    }
                }
            }
            catch (Exception ex)
            {
                // Không re-throw — đây là side-effect, lỗi không được lan lên KnowledgeBaseService
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("UploadKnowledgeBaseFileAsync — file: {0}{1}{2}", fileName, Environment.NewLine, ex));
            }
        }

        /// <inheritdoc/>
        public async Task<string> EnsureKnowledgeBaseNotebookAsync()
        {
            // Fast path: cache còn hạn
            if (!string.IsNullOrWhiteSpace(_cachedNotebookId) && DateTime.UtcNow < _cacheExpiry)
                return _cachedNotebookId;

            // Chỉ cho 1 thread vào để tránh race condition tạo notebook trùng
            await _notebookLock.WaitAsync();
            try
            {
                // Double-check sau khi acquire lock
                if (!string.IsNullOrWhiteSpace(_cachedNotebookId) && DateTime.UtcNow < _cacheExpiry)
                    return _cachedNotebookId;

                var baseUrl = GetBaseUrl();
                if (baseUrl == null) return null;

                var notebookId = await FindOrCreateNotebookAsync(baseUrl, KNOWLEDGE_BASE_NOTEBOOK);

                if (!string.IsNullOrWhiteSpace(notebookId))
                {
                    _cachedNotebookId = notebookId;
                    _cacheExpiry      = DateTime.UtcNow.Add(_cacheTtl);
                }

                return notebookId;
            }
            finally
            {
                _notebookLock.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckConnectionAsync()
        {
            var baseUrl = GetBaseUrl();
            if (baseUrl == null) return false;

            try
            {
                var req = BuildRequest(HttpMethod.Get, baseUrl + HEALTH_ENDPOINT);
                var res = await _httpClient.SendAsync(req);
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    string.Format("CheckConnectionAsync: {0}", ex.Message));
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<List<OpenNotebookModelItem>> GetLanguageModelsAsync()
        {
            var result = new List<OpenNotebookModelItem>();

            var baseUrl = GetBaseUrl();
            if (baseUrl == null)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    "GetLanguageModelsAsync: OPEN_NOTEBOOK_BASE_URL chưa cấu hình.");
                return result;
            }

            try
            {
                var req = BuildRequest(HttpMethod.Get, baseUrl + MODELS_ENDPOINT + "?type=language");
                var res = await _httpClient.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        string.Format("GetLanguageModelsAsync: HTTP {0}", (int)res.StatusCode));
                    return result;
                }

                var body = await res.Content.ReadAsStringAsync();
                var arr  = JArray.Parse(body);

                foreach (var item in arr)
                {
                    var id   = item["id"]?.ToString();
                    var type = item["type"]?.ToString() ?? "";
                    var name = item["name"]?.ToString() ?? id;

                    if (string.IsNullOrWhiteSpace(id)
                        || !id.StartsWith("model:", StringComparison.OrdinalIgnoreCase)
                        || !type.Equals("language", StringComparison.OrdinalIgnoreCase))
                        continue;

                    result.Add(new OpenNotebookModelItem { Id = id, Name = name });
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("GetLanguageModelsAsync{0}{1}", Environment.NewLine, ex));
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetDefaultLanguageModelIdAsync()
        {
            await _modelLock.WaitAsync();
            try
            {
                var baseUrl = GetBaseUrl();
                if (baseUrl == null) return null;

                // Gọi GET /api/models/defaults lấy default model từ Open Notebook
                try
                {
                    var req = BuildRequest(HttpMethod.Get, baseUrl + MODELS_DEFAULTS_ENDPOINT);
                    var res = await _httpClient.SendAsync(req);

                    if (res.IsSuccessStatusCode)
                    {
                        var body = await res.Content.ReadAsStringAsync();
                        var json = JObject.Parse(body);

                        // Open Notebook trả về object với các key như "default_chat_model", "default_embedding_model", v.v.
                        // Lấy field "default_chat_model" là default model cho hỏi đáp
                        var languageModel = json["default_chat_model"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(languageModel) &&
                            languageModel.StartsWith("model:", StringComparison.OrdinalIgnoreCase))
                        {
                            _cachedDefaultModelId = languageModel;
                            _modelCacheExpiry     = DateTime.UtcNow.Add(_cacheTtl);
                            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.OpenNotebook.ToString(),
                                string.Format("GetDefaultLanguageModelId: default model từ Open Notebook = '{0}'", languageModel));
                            return _cachedDefaultModelId;
                        }
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                            string.Format("GetDefaultLanguageModelId: GET /api/models/defaults HTTP {0}", (int)res.StatusCode));
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        string.Format("GetDefaultLanguageModelId: lỗi gọi /api/models/defaults — {0}", ex.Message));
                }

                return null;
            }
            finally
            {
                _modelLock.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<OpenNotebookAskResponse>> AskKnowledgeBaseAsync(
            OpenNotebookAskRequest request)
        {
            var response = new ResponseModel<OpenNotebookAskResponse>();

            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Question không được để trống.");
                return response;
            }

            var baseUrl = GetBaseUrl();
            if (baseUrl == null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("OPEN_NOTEBOOK_BASE_URL chưa được cấu hình.");
                return response;
            }

            try
            {
                // Bước 1: Lấy notebook ID
                var notebookId = await EnsureKnowledgeBaseNotebookAsync();
                if (string.IsNullOrWhiteSpace(notebookId))
                {
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    response.ErrorMessages.Add("Không thể kết nối tới Open Notebook hoặc tạo notebook KnowledgeBase.");
                    return response;
                }

                // Bước 2: Resolve model IDs
                // Open Notebook yêu cầu cả 3 field bắt buộc dạng SurrealDB record ID "model:xxx".
                // Ưu tiên: request override → OPEN_NOTEBOOK_DEFAULT_MODEL config → tự động lấy từ /api/models
                var defaultModelId   = await GetDefaultLanguageModelIdAsync();
                var strategyModel    = ResolveValidModelId(request.StrategyModel,    AppSettings.OPEN_NOTEBOOK_DEFAULT_MODEL) ?? defaultModelId;
                var answerModel      = ResolveValidModelId(request.AnswerModel,      AppSettings.OPEN_NOTEBOOK_DEFAULT_MODEL) ?? defaultModelId;
                var finalAnswerModel = ResolveValidModelId(request.FinalAnswerModel, AppSettings.OPEN_NOTEBOOK_DEFAULT_MODEL) ?? defaultModelId;

                if (string.IsNullOrWhiteSpace(strategyModel))
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.ErrorMessages.Add(
                        "Không tìm được language model trên Open Notebook. " +
                        "Hãy cấu hình OPEN_NOTEBOOK_DEFAULT_MODEL=model:xxx, " +
                        "hoặc thêm ít nhất 1 language model trong Open Notebook > Settings > Models.");
                    return response;
                }

                // Bước 3: Build request body đầy đủ 3 field bắt buộc
                var body = new JObject
                {
                    ["question"]           = request.Question,
                    ["strategy_model"]     = strategyModel,
                    ["answer_model"]       = answerModel,
                    ["final_answer_model"] = finalAnswerModel,
                    ["notebook_id"]        = notebookId,
                };

                var req = BuildRequest(HttpMethod.Post, baseUrl + ASK_SIMPLE_ENDPOINT);
                req.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");

                // Ask có thể mất vài chục giây — dùng timeout riêng
                var cts        = new System.Threading.CancellationTokenSource(
                    TimeSpan.FromSeconds(AppSettings.OPEN_NOTEBOOK_ASK_TIMEOUT_SECONDS));
                var httpRes    = await _httpClient.SendAsync(req, cts.Token);
                var httpBody   = await httpRes.Content.ReadAsStringAsync();

                if (!httpRes.IsSuccessStatusCode)
                {
                    response.StatusCode = (int)httpRes.StatusCode;
                    response.ErrorMessages.Add(string.Format(
                        "Open Notebook trả về HTTP {0}: {1}",
                        (int)httpRes.StatusCode, TruncateMessage(httpBody, 400)));
                    return response;
                }

                var json = JObject.Parse(httpBody);
                var rawAnswer = json["answer"]?.ToString() ?? string.Empty;

                response.Data = new OpenNotebookAskResponse
                {
                    Question   = json["question"]?.ToString() ?? request.Question,
                    Answer     = ReplaceSourceIdsInAnswer(rawAnswer),
                    NotebookId = notebookId,
                };
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                response.ErrorMessages.Add(string.Format(
                    "Open Notebook không phản hồi sau {0} giây. Vui lòng thử lại.",
                    AppSettings.OPEN_NOTEBOOK_ASK_TIMEOUT_SECONDS));
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("AskKnowledgeBaseAsync — question: {0}{1}{2}",
                        request.Question, Environment.NewLine, ex));
            }

            return response;
        }

        // =====================================================================
        // Private — notebook management
        // =====================================================================

        /// <summary>
        /// Gọi GET /api/notebooks để tìm notebook theo tên.
        /// Nếu không tìm thấy thì tạo mới qua POST /api/notebooks.
        /// Trả về notebook ID, hoặc null nếu thất bại.
        /// </summary>
        private async Task<string> FindOrCreateNotebookAsync(string baseUrl, string notebookName)
        {
            try
            {
                // GET /api/notebooks
                var req = BuildRequest(HttpMethod.Get, baseUrl + NOTEBOOKS_ENDPOINT);
                var res = await _httpClient.SendAsync(req);
                var body = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode)
                {
                    var list = JArray.Parse(body);
                    foreach (var nb in list)
                    {
                        var name = nb["name"]?.ToString();
                        if (string.Equals(name, notebookName, StringComparison.OrdinalIgnoreCase))
                        {
                            var existingId = nb["id"]?.ToString();
                            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.OpenNotebook.ToString(),
                                string.Format("FindOrCreateNotebook: Tìm thấy notebook '{0}', id={1}",
                                    notebookName, existingId));
                            return existingId;
                        }
                    }
                }
                else
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        string.Format("FindOrCreateNotebook: GET /api/notebooks HTTP {0}", (int)res.StatusCode));
                }

                // Chưa tồn tại → tạo mới
                return await CreateNotebookAsync(baseUrl, notebookName);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("FindOrCreateNotebook — {0}{1}{2}", notebookName, Environment.NewLine, ex));
                return null;
            }
        }

        /// <summary>
        /// POST /api/notebooks — tạo notebook mới và trả về ID.
        /// </summary>
        private async Task<string> CreateNotebookAsync(string baseUrl, string notebookName)
        {
            try
            {
                var payload = new JObject
                {
                    ["name"]        = notebookName,
                    ["description"] = "Tài liệu quy trình vận hành từ Elogbook KnowledgeBase",
                };

                var req = BuildRequest(HttpMethod.Post, baseUrl + NOTEBOOKS_ENDPOINT);
                req.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

                var res  = await _httpClient.SendAsync(req);
                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                        string.Format("CreateNotebook: HTTP {0} — {1}", (int)res.StatusCode,
                            TruncateMessage(body, 200)));
                    return null;
                }

                var json       = JObject.Parse(body);
                var newId      = json["id"]?.ToString();

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.OpenNotebook.ToString(),
                    string.Format("CreateNotebook: Đã tạo notebook '{0}', id={1}", notebookName, newId));

                return newId;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OpenNotebook.ToString(),
                    string.Format("CreateNotebook — {0}{1}{2}", notebookName, Environment.NewLine, ex));
                return null;
            }
        }

        // =====================================================================
        // Private — upload helpers
        // =====================================================================

        /// <summary>
        /// Upload một file dưới dạng multipart/form-data tới POST /api/sources.
        /// Sau đó link source vào notebook nếu NotebookId được chỉ định.
        /// </summary>
        private async Task<OpenNotebookUploadResult> UploadSingleFileAsync(
            string baseUrl, OpenNotebookUploadItem file)
        {
            var result = new OpenNotebookUploadResult { FileName = file.FileName };

            var fileBytes    = ReadStream(file.Content);
            var displayTitle = string.IsNullOrWhiteSpace(file.Title) ? file.FileName : file.Title;

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent("upload"),  "type");
                form.Add(new StringContent(displayTitle), "title");
                form.Add(new StringContent("true"),    "embed");
                form.Add(new StringContent("true"),    "async_processing");
                form.Add(new StringContent("true"),    "delete_source"); // Xóa file vật lý sau khi content-core đọc xong, giữ nguyên embeddings

                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.MimeType);
                form.Add(fileContent, "file", file.FileName);

                var req = BuildRequest(HttpMethod.Post, baseUrl + SOURCES_ENDPOINT);
                req.Content = form;

                var res  = await _httpClient.SendAsync(req);
                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    result.IsError      = true;
                    result.ErrorMessage = string.Format("HTTP {0}: {1}",
                        (int)res.StatusCode, TruncateMessage(body, 300));
                    return result;
                }

                var json = JObject.Parse(body);
                result.SourceId  = json["id"]?.ToString();
                result.Title     = json["title"]?.ToString();
                result.Status    = json["status"]?.ToString() ?? "new";
                result.CommandId = json["command_id"]?.ToString();

                if (string.IsNullOrWhiteSpace(result.SourceId))
                {
                    result.IsError      = true;
                    result.ErrorMessage = "Open Notebook không trả về source ID.";
                    return result;
                }
            }

            // Link source vào notebook
            if (!string.IsNullOrWhiteSpace(file.NotebookId) && !result.IsError)
                await LinkSourceToNotebookAsync(baseUrl, file.NotebookId, result.SourceId);

            return result;
        }

        /// <summary>
        /// POST /api/notebooks/{notebookId}/sources/{sourceId}
        /// Lỗi chỉ ghi log, không ảnh hưởng kết quả upload.
        /// </summary>
        private async Task LinkSourceToNotebookAsync(string baseUrl, string notebookId, string sourceId)
        {
            try
            {
                var endpoint = string.Format(LINK_SOURCE_ENDPOINT, notebookId, sourceId);
                var req      = BuildRequest(HttpMethod.Post, baseUrl + endpoint);
                req.Content  = new StringContent("{}", Encoding.UTF8, "application/json");

                var res = await _httpClient.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                        string.Format("LinkSource: source={0}, notebook={1} — HTTP {2}: {3}",
                            sourceId, notebookId, (int)res.StatusCode, TruncateMessage(body, 200)));
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    string.Format("LinkSource: source={0}, notebook={1}{2}{3}",
                        sourceId, notebookId, Environment.NewLine, ex));
            }
        }

        // =====================================================================
        // Private — utilities
        // =====================================================================

        private static string GetBaseUrl()
        {
            var url = AppSettings.OPEN_NOTEBOOK_BASE_URL;
            return string.IsNullOrWhiteSpace(url) ? null : url.TrimEnd('/');
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, string url)
        {
            var req = new HttpRequestMessage(method, url);
            var pwd = AppSettings.OPEN_NOTEBOOK_PASSWORD;
            if (!string.IsNullOrWhiteSpace(pwd))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pwd);
            return req;
        }

        private static byte[] ReadStream(Stream stream)
        {
            if (stream is MemoryStream ms) return ms.ToArray();
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                return mem.ToArray();
            }
        }

        /// <summary>
        /// Trả về MIME type dựa trên extension file.
        /// Fallback về application/octet-stream nếu không nhận dạng được.
        /// </summary>
        private static string ResolveMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            switch (ext)
            {
                case ".pdf":  return "application/pdf";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".doc":  return "application/msword";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xls":  return "application/vnd.ms-excel";
                case ".txt":  return "text/plain";
                case ".html":
                case ".htm":  return "text/html";
                case ".xml":  return "application/xml";
                case ".json": return "application/json";
                case ".csv":  return "text/csv";
                case ".md":   return "text/markdown";
                default:      return "application/octet-stream";
            }
        }

        private static string TruncateMessage(string msg, int max)
        {
            if (string.IsNullOrEmpty(msg) || msg.Length <= max) return msg;
            return msg.Substring(0, max) + "...";
        }

        /// <summary>
        /// Trả về model ID nếu hợp lệ (dạng "model:xxx" — SurrealDB record ID).
        /// Giá trị không hợp lệ (tên model thuần như "gemini-3.6-flash") trả về null
        /// để caller không include vào request body, cho Open Notebook tự dùng default.
        /// </summary>
        private static string ResolveValidModelId(string overrideValue, string defaultValue)
        {
            var candidate = !string.IsNullOrWhiteSpace(overrideValue) ? overrideValue : defaultValue;
            if (string.IsNullOrWhiteSpace(candidate)) return null;

            // Open Notebook model IDs phải có dạng "model:xxx" (SurrealDB record ID)
            return candidate.StartsWith("model:", StringComparison.OrdinalIgnoreCase)
                ? candidate
                : null;  // tên model thuần → bỏ qua để server dùng default
        }

        /// <summary>
        /// Tìm tất cả source ID dạng "source:xxx" trong answer,
        /// query DB lấy FileName từ bảng Attachment (theo OpenNotebookSourceId),
        /// rồi replace trong text.
        ///
        /// Pattern nhận dạng: [source:xxx] hoặc source:xxx (standalone)
        /// Kết quả: thay bằng tên file — VD: [bao-cao-giao-ca.pdf]
        /// Nếu không tìm thấy mapping → giữ nguyên để không mất thông tin.
        /// </summary>
        private string ReplaceSourceIdsInAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) return answer;

            // Tìm tất cả source ID unique trong answer
            var matches = Regex.Matches(answer, @"\[?(source:[a-z0-9]+)\]?",
                RegexOptions.IgnoreCase);

            if (matches.Count == 0) return answer;

            var sourceIds = matches
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Batch query DB một lần
            Dictionary<string, string> sourceMap;
            try
            {
                var attachments = _unitOfWork
                    .ElogbookRepository<Attachment>()
                    .GetQuery(x => !x.IsDeleted
                        && x.OpenNotebookSourceId != null
                        && sourceIds.Contains(x.OpenNotebookSourceId))
                    .Select(x => new { x.OpenNotebookSourceId, x.FileName })
                    .ToList();

                sourceMap = attachments.ToDictionary(
                    x => x.OpenNotebookSourceId,
                    x => x.FileName,
                    StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, Category.OpenNotebook.ToString(),
                    string.Format("ReplaceSourceIdsInAnswer: lỗi query DB — {0}", ex.Message));
                return answer;
            }

            if (sourceMap.Count == 0) return answer;

            // Replace trong answer
            return Regex.Replace(
                answer,
                @"\[?(source:[a-z0-9]+)\]?",
                m =>
                {
                    var id = m.Groups[1].Value;
                    return sourceMap.TryGetValue(id, out var fileName)
                        ? string.Format("[{0}]", fileName)
                        : m.Value; // giữ nguyên nếu không tìm thấy
                },
                RegexOptions.IgnoreCase);
        }
    }
}
