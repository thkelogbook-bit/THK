namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin một language model lấy từ Open Notebook GET /api/models
    /// </summary>
    public class OpenNotebookModelItem
    {
        /// <summary>
        /// SurrealDB record ID — dạng "model:abc123".
        /// Dùng để truyền vào strategyModel / answerModel / finalAnswerModel khi gọi ask.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tên hiển thị của model (VD: "openrouter/free", "gemini-1.5-flash")
        /// </summary>
        public string Name { get; set; }
    }
}
