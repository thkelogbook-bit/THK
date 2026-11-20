using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Service tích hợp Gemini AI cho KnowledgeBase
    /// </summary>
    public interface IKnowledgeBaseAiService
    {
        /// <summary>
        /// Upload nhiều file lên Gemini Files API và trả về danh sách file URI
        /// </summary>
        Task<ResponseModel<List<GeminiFileUploadResult>>> UploadFilesAsync(
            IEnumerable<KnowledgeBaseAiUploadItem> files);

        /// <summary>
        /// Query AI với text, có thể kèm danh sách fileUri trong request.FileUris
        /// </summary>
        Task<ResponseModel<KnowledgeBaseAiQueryResponse>> QueryAsync(
            KnowledgeBaseAiQueryRequest request);
    }
}
