using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Controller tích hợp Gemini AI — upload file và query tài liệu
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/knowledgeBaseAi")]
    public class KnowledgeBaseAiController : BaseApiController
    {
        private readonly IKnowledgeBaseAiBusiness _knowledgeBaseAiBusiness;

        public KnowledgeBaseAiController(IKnowledgeBaseAiBusiness knowledgeBaseAiBusiness)
        {
            _knowledgeBaseAiBusiness = knowledgeBaseAiBusiness;
        }

        /// <summary>
        /// Query AI với câu hỏi dạng text, có thể kèm danh sách fileUri đã upload trước.
        /// FileUris lấy từ response của POST /uploadFiles.
        /// </summary>
        [Route("query")]
        [HttpPost]
        public async Task<ResponseModel<KnowledgeBaseAiQueryResponse>> Query([FromBody] KnowledgeBaseAiQueryRequest request)
        {
            return await _knowledgeBaseAiBusiness.QueryAsync(request);
        }

        /// <summary>
        /// Upload nhiều file (multipart/form-data) lên Gemini Files API.
        /// Response trả về danh sách fileUri để dùng khi query có context tài liệu.
        /// </summary>
        [Route("uploadFiles")]
        [HttpPost]
        public async Task<ResponseModel<List<GeminiFileUploadResult>>> UploadFiles()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                var errorResp = new ResponseModel<List<GeminiFileUploadResult>>();
                errorResp.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                errorResp.ErrorMessages.Add("Request phải là multipart/form-data.");
                return errorResp;
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileInputs = new List<KnowledgeBaseAiUploadItem>();

            foreach (var filePart in provider.Contents)
            {
                var disposition = filePart.Headers.ContentDisposition;
                if (disposition == null || disposition.FileName == null) continue;

                var fileName = disposition.FileName.Trim('"');
                var mimeType = filePart.Headers.ContentType != null
                    ? filePart.Headers.ContentType.MediaType
                    : GetMimeTypeFromExtension(fileName);

                var stream = await filePart.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                fileInputs.Add(new KnowledgeBaseAiUploadItem
                {
                    FileName = fileName,
                    Content = memoryStream,
                    MimeType = mimeType
                });
            }

            if (fileInputs.Count == 0)
            {
                var errorResp = new ResponseModel<List<GeminiFileUploadResult>>();
                errorResp.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResp.ErrorMessages.Add("Không tìm thấy file nào trong request.");
                return errorResp;
            }

            return await _knowledgeBaseAiBusiness.UploadFilesAsync(fileInputs);
        }

        private static string GetMimeTypeFromExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (ext == null) return "application/octet-stream";
            switch (ext.ToLowerInvariant())
            {
                case ".pdf":  return "application/pdf";
                case ".doc":  return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":  return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":  return "text/plain";
                case ".png":  return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                default:      return "application/octet-stream";
            }
        }
    }
}
