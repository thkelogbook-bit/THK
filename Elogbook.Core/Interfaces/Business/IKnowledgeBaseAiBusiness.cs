using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Business tích hợp Gemini AI cho KnowledgeBase
    /// </summary>
    public interface IKnowledgeBaseAiBusiness
    {
        /// <summary>
        /// Upload nhiều file lên Gemini Files API
        /// </summary>
        Task<ResponseModel<List<GeminiFileUploadResult>>> UploadFilesAsync(
            IEnumerable<KnowledgeBaseAiUploadItem> files);

        /// <summary>
        /// Query AI với câu hỏi text, có thể kèm fileUri đã upload trước (trong request.FileUris)
        /// </summary>
        Task<ResponseModel<KnowledgeBaseAiQueryResponse>> QueryAsync(
            KnowledgeBaseAiQueryRequest request);
    }
}
