namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kết quả hỏi AI trong notebook KnowledgeBase của Open Notebook
    /// </summary>
    public class OpenNotebookAskResponse
    {
        /// <summary>
        /// Câu hỏi gốc đã gửi đi
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Câu trả lời tổng hợp từ AI dựa trên nội dung các tài liệu trong notebook KnowledgeBase
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// ID notebook KnowledgeBase đã được dùng để tìm kiếm
        /// </summary>
        public string NotebookId { get; set; }
    }
}
