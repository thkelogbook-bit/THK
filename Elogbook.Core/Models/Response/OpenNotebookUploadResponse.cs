namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kết quả upload một file lên Open Notebook
    /// </summary>
    public class OpenNotebookUploadResult
    {
        /// <summary>
        /// Tên file gốc được upload
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Source ID được Open Notebook cấp sau khi upload thành công
        /// Ví dụ: "source:abc123xyz"
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// Tiêu đề source trong Open Notebook
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Trạng thái xử lý source phía Open Notebook ("new", "queued", "running", "completed", "failed")
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Command ID theo dõi quá trình xử lý bất đồng bộ (embed, index...)
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// Upload có lỗi không
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Thông báo lỗi nếu IsError = true
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
