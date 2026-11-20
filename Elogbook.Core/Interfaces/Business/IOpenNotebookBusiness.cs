using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Business tích hợp Open Notebook — hỏi AI trong notebook KnowledgeBase
    /// </summary>
    public interface IOpenNotebookBusiness
    {
        /// <summary>
        /// Lấy model mặc định đang được cấu hình trên Open Notebook.
        /// </summary>
        Task<ResponseModel<OpenNotebookModelItem>> GetDefaultModelAsync();

        /// <summary>
        /// Lấy danh sách language model từ Open Notebook — dùng cho dropdown chọn model.
        /// </summary>
        Task<ResponseModel<List<OpenNotebookModelItem>>> GetLanguageModelsAsync();

        /// <summary>
        /// Gửi câu hỏi tới Open Notebook ask workflow, tìm kiếm trong notebook "KnowledgeBase".
        /// Validation + exception wrap + gọi service.
        /// </summary>
        Task<ResponseModel<OpenNotebookAskResponse>> AskKnowledgeBaseAsync(
            OpenNotebookAskRequest request);

        /// <summary>
        /// Kiểm tra kết nối tới Open Notebook API và trạng thái notebook KnowledgeBase.
        /// </summary>
        Task<ResponseModel<bool>> CheckConnectionAsync();
    }
}
