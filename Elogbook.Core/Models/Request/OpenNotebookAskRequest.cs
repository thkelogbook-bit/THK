namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request hỏi AI trong notebook KnowledgeBase của Open Notebook.
    /// Open Notebook sẽ tự tìm kiếm semantic trong các tài liệu đã upload
    /// và tổng hợp câu trả lời qua LangGraph ask workflow.
    /// </summary>
    public class OpenNotebookAskRequest
    {
        /// <summary>
        /// Câu hỏi cần hỏi (bắt buộc).
        /// Ví dụ: "Quy trình vận hành máy phát điện khi xảy ra sự cố quá tải là gì?"
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Model ID dùng cho bước lên chiến lược tìm kiếm.
        /// Nếu để trống, service sẽ dùng model mặc định cấu hình trong appSettings.
        /// Ví dụ: "model:abc123" (SurrealDB record ID trong Open Notebook)
        /// </summary>
        public string StrategyModel { get; set; }

        /// <summary>
        /// Model ID dùng để tổng hợp từng câu trả lời thành phần.
        /// Nếu để trống, dùng model mặc định.
        /// </summary>
        public string AnswerModel { get; set; }

        /// <summary>
        /// Model ID dùng để viết câu trả lời cuối cùng.
        /// Nếu để trống, dùng model mặc định.
        /// </summary>
        public string FinalAnswerModel { get; set; }
    }
}
