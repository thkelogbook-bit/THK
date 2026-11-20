using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request model cho query AI với text và danh sách file URI tùy chọn
    /// </summary>
    public class KnowledgeBaseAiQueryRequest
    {
        /// <summary>
        /// Câu hỏi / nội dung query gửi đến AI
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Danh sách file URI đã upload qua /uploadFiles (tùy chọn).
        /// Ví dụ: ["https://generativelanguage.googleapis.com/v1beta/files/abc123"]
        /// AI sẽ dùng nội dung các file này làm context khi trả lời.
        /// </summary>
        public List<string> FileUris { get; set; }
    }
}
