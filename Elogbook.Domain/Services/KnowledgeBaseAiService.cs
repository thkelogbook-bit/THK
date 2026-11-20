using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Tích hợp Gemini Files API và generateContent API
    /// </summary>
    public class KnowledgeBaseAiService : IKnowledgeBaseAiService
    {
        private const string UPLOAD_BASE_URL = "https://generativelanguage.googleapis.com/upload/v1beta/files";

        private static string GenerateBaseUrl
        {
            get
            {
                var model = AppSettings.GEMINI_MODEL;
                return string.Format("https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent", model);
            }
        }

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        /// <inheritdoc/>
        public async Task<ResponseModel<List<GeminiFileUploadResult>>> UploadFilesAsync(
            IEnumerable<KnowledgeBaseAiUploadItem> files)
        {
            var response = new ResponseModel<List<GeminiFileUploadResult>>();
            response.Data = new List<GeminiFileUploadResult>();

            var apiKey = AppSettings.GEMINI_API_KEY;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("GEMINI_API_KEY chưa được cấu hình.");
                return response;
            }

            foreach (var file in files)
            {
                var result = new GeminiFileUploadResult
                {
                    FileName = file.FileName,  // giữ tên gốc để hiển thị trên UI
                    MimeType = file.MimeType
                };

                try
                {
                    var uploadUrl = string.Format("{0}?key={1}", UPLOAD_BASE_URL, apiKey);
                    var fileBytes = ReadStream(file.Content);

                    // Header HTTP không chấp nhận ký tự non-ASCII
                    // Encode tên file sang ASCII-safe (URL encode)
                    var safeFileName = Uri.EscapeDataString(file.FileName);

                    using (var content = new ByteArrayContent(fileBytes))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue(file.MimeType);
                        content.Headers.Add("X-Goog-Upload-File-Name", safeFileName);
                        content.Headers.Add("X-Goog-Upload-Protocol", "raw");

                        var httpResponse = await _httpClient.PostAsync(uploadUrl, content);
                        var responseBody = await httpResponse.Content.ReadAsStringAsync();

                        if (httpResponse.IsSuccessStatusCode)
                        {
                            var json = JObject.Parse(responseBody);
                            result.FileUri = json["file"] != null ? json["file"]["uri"] != null ? json["file"]["uri"].ToString() : null : null;
                            result.IsError = string.IsNullOrWhiteSpace(result.FileUri);
                            if (result.IsError)
                            {
                                result.ErrorMessage = "Không lấy được file URI từ response Gemini.";
                            }
                        }
                        else
                        {
                            result.IsError = true;
                            result.ErrorMessage = string.Format("HTTP {0}: {1}", (int)httpResponse.StatusCode, responseBody);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IsError = true;
                    result.ErrorMessage = ex.Message;
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.KnowledgeBaseAiBusiness.ToString(),
                        string.Format("UploadFilesAsync — file: {0}{1}{2}", file.FileName, Environment.NewLine, ex));
                }

                response.Data.Add(result);
            }

            return response;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<KnowledgeBaseAiQueryResponse>> QueryAsync(
            KnowledgeBaseAiQueryRequest request)
        {
            var response = new ResponseModel<KnowledgeBaseAiQueryResponse>();

            if (request == null || string.IsNullOrWhiteSpace(request.Query))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Query không được để trống.");
                return response;
            }

            var apiKey = AppSettings.GEMINI_API_KEY;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("GEMINI_API_KEY chưa được cấu hình.");
                return response;
            }

            try
            {
                var parts = new JArray();

                // Thêm file context nếu có
                if (request.FileUris != null)
                {
                    foreach (var uri in request.FileUris)
                    {
                        if (string.IsNullOrWhiteSpace(uri))
                            continue;

                        parts.Add(new JObject
                        {
                            ["file_data"] = new JObject
                            {
                                ["file_uri"] = uri
                            }
                        });
                    }
                }

                parts.Add(new JObject { ["text"] = request.Query });

                var body = new JObject
                {
                    ["contents"] = new JArray
                    {
                        new JObject { ["parts"] = parts }
                    }
                };

                var requestUrl = string.Format("{0}?key={1}", GenerateBaseUrl, apiKey);
                var httpContent = new StringContent(body.ToString(Formatting.None), Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync(requestUrl, httpContent);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    var json = JObject.Parse(responseBody);
                    var text = json["candidates"] != null
                        && json["candidates"][0] != null
                        && json["candidates"][0]["content"] != null
                        && json["candidates"][0]["content"]["parts"] != null
                        && json["candidates"][0]["content"]["parts"][0] != null
                        ? json["candidates"][0]["content"]["parts"][0]["text"] != null
                            ? json["candidates"][0]["content"]["parts"][0]["text"].ToString()
                            : string.Empty
                        : string.Empty;

                    response.Data = new KnowledgeBaseAiQueryResponse { Answer = text };
                }
                else
                {
                    response.StatusCode = (int)httpResponse.StatusCode;
                    response.ErrorMessages.Add(string.Format("Gemini API lỗi: {0}", responseBody));
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.KnowledgeBaseAiBusiness.ToString(),
                    string.Format("QueryAsync — query: {0}{1}{2}", request.Query, Environment.NewLine, ex));
            }

            return response;
        }

        private static byte[] ReadStream(Stream stream)
        {
            var ms = stream as MemoryStream;
            if (ms != null)
                return ms.ToArray();

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
