using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Tích hợp Open Notebook — hỏi AI trong notebook KnowledgeBase.
    /// Tài liệu được index vào Open Notebook khi upload qua màn hình Danh mục tài liệu.
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/openNotebook")]
    public class OpenNotebookController : BaseApiController
    {
        private readonly IOpenNotebookBusiness _openNotebookBusiness;

        public OpenNotebookController(IOpenNotebookBusiness openNotebookBusiness)
        {
            _openNotebookBusiness = openNotebookBusiness;
        }

        /// <summary>
        /// Lấy model ID mặc định từ Open Notebook (ưu tiên OPEN_NOTEBOOK_DEFAULT_MODEL config,
        /// fallback tự động lấy model language đầu tiên từ /api/models).
        /// Dùng để hiển thị model đang được dùng trên UI.
        /// </summary>
        [Route("defaultModel")]
        [HttpGet]
        public async Task<ResponseModel<OpenNotebookModelItem>> GetDefaultModel()
        {
            return await _openNotebookBusiness.GetDefaultModelAsync();
        }

        /// <summary>
        /// Lấy danh sách language model từ Open Notebook.
        /// Dùng để hiển thị dropdown cho user tự chọn model khi hỏi đáp AI.
        /// </summary>
        [Route("models")]
        [HttpGet]
        public async Task<ResponseModel<List<OpenNotebookModelItem>>> GetLanguageModels()
        {
            return await _openNotebookBusiness.GetLanguageModelsAsync();
        }

        /// <summary>
        /// Hỏi AI dựa trên các tài liệu đã upload vào notebook "KnowledgeBase".
        ///
        /// Open Notebook sẽ:
        ///   1. Lên chiến lược tìm kiếm từ câu hỏi
        ///   2. Tìm kiếm semantic trong các tài liệu KnowledgeBase đã embed
        ///   3. Tổng hợp câu trả lời qua LangGraph ask workflow
        ///
        /// Lưu ý: endpoint này có thể mất 10–60 giây tùy model và câu hỏi.
        /// Client nên set timeout tối thiểu 120 giây.
        /// </summary>
        /// <param name="request">
        /// - question (bắt buộc): câu hỏi cần hỏi
        /// - strategyModel, answerModel, finalAnswerModel (tùy chọn): override model ID
        ///   trong Open Notebook. Nếu để trống, dùng OPEN_NOTEBOOK_DEFAULT_MODEL từ config.
        ///   Model ID có dạng "model:abc123" — lấy từ GET /api/models trên Open Notebook.
        /// </param>
        [Route("ask")]
        [HttpPost]
        public async Task<ResponseModel<OpenNotebookAskResponse>> Ask(
            [FromBody] OpenNotebookAskRequest request)
        {
            return await _openNotebookBusiness.AskKnowledgeBaseAsync(request);
        }

        /// <summary>
        /// Kiểm tra kết nối tới Open Notebook API.
        /// Dùng để xác nhận cấu hình OPEN_NOTEBOOK_BASE_URL và password đúng.
        /// </summary>
        [Route("checkConnection")]
        [HttpGet]
        public async Task<ResponseModel<bool>> CheckConnection()
        {
            return await _openNotebookBusiness.CheckConnectionAsync();
        }
    }
}
