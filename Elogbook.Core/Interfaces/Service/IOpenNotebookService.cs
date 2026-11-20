using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Service tích hợp với Open Notebook — upload file tài liệu từ Elogbook
    /// sang Open Notebook để làm nguồn cho AI research assistant.
    /// </summary>
    public interface IOpenNotebookService
    {
        /// <summary>
        /// Upload một hoặc nhiều file lên Open Notebook dưới dạng sources.
        /// Mỗi file được tạo thành một source độc lập trong Open Notebook.
        /// Nếu <see cref="OpenNotebookUploadItem.NotebookId"/> được cung cấp,
        /// source sẽ được gắn vào notebook tương ứng sau khi tạo.
        /// </summary>
        Task<ResponseModel<List<OpenNotebookUploadResult>>> UploadFilesAsync(
            IEnumerable<OpenNotebookUploadItem> files);

        /// <summary>
        /// Upload một file tài liệu KnowledgeBase lên Open Notebook và trả về SourceId.
        /// Tự động resolve notebook "KnowledgeBase". Fire-and-forget phía caller nhưng awaitable.
        /// </summary>
        /// <returns>Source ID dạng "source:xxx", hoặc null nếu thất bại</returns>
        Task<string> UploadKnowledgeBaseFileAndGetIdAsync(string filePath, string fileName, string title);

        /// <summary>
        /// Upload một file tài liệu KnowledgeBase lên Open Notebook.
        /// Tự động resolve notebook "KnowledgeBase" (tạo mới nếu chưa tồn tại).
        /// Fire-and-forget: trả về ngay, không chờ Open Notebook xử lý xong.
        /// </summary>
        /// <param name="filePath">Đường dẫn tuyệt đối tới file trên server</param>
        /// <param name="fileName">Tên file gốc để hiển thị</param>
        /// <param name="title">Tiêu đề tài liệu (ví dụ: ProcessDocumentCode + Title)</param>
        Task UploadKnowledgeBaseFileAsync(string filePath, string fileName, string title);

        /// <summary>
        /// Lấy (hoặc tạo mới nếu chưa có) notebook tên "KnowledgeBase" trong Open Notebook.
        /// Kết quả được cache trong bộ nhớ — chỉ gọi API lần đầu hoặc khi cache hết hạn.
        /// </summary>
        /// <returns>Notebook ID trong Open Notebook, hoặc null nếu không kết nối được</returns>
        Task<string> EnsureKnowledgeBaseNotebookAsync();

        /// <summary>
        /// Gửi câu hỏi tới Open Notebook ask workflow, giới hạn tìm kiếm
        /// trong notebook "KnowledgeBase". Trả về câu trả lời tổng hợp từ AI.
        /// Dùng endpoint POST /search/ask/simple (non-streaming).
        /// </summary>
        Task<ResponseModel<OpenNotebookAskResponse>> AskKnowledgeBaseAsync(
            OpenNotebookAskRequest request);

        /// <summary>
        /// Lấy model ID ngôn ngữ (language modality) đầu tiên từ Open Notebook.
        /// Cache 1 giờ. Dùng làm fallback khi OPEN_NOTEBOOK_DEFAULT_MODEL không cấu hình hoặc sai format.
        /// </summary>
        Task<string> GetDefaultLanguageModelIdAsync();

        /// <summary>
        /// Lấy toàn bộ danh sách language model từ Open Notebook GET /api/models?type=language.
        /// Dùng để hiển thị dropdown cho user tự chọn model khi hỏi đáp AI.
        /// </summary>
        Task<List<OpenNotebookModelItem>> GetLanguageModelsAsync();

        /// <summary>
        /// Kiểm tra kết nối tới Open Notebook API.
        /// </summary>
        Task<bool> CheckConnectionAsync();
    }
}
