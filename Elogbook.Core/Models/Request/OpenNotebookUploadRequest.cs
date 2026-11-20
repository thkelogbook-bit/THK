using System.IO;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin một file cần upload lên Open Notebook
    /// </summary>
    public class OpenNotebookUploadItem
    {
        /// <summary>
        /// Tên file gốc (ví dụ: "QuyTrinhVanHanh.pdf")
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// MIME type của file (ví dụ: "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Nội dung file dưới dạng stream
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// Tiêu đề hiển thị trong Open Notebook (nếu null sẽ dùng FileName)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ID notebook đích trong Open Notebook (nếu null thì upload source độc lập, không gắn vào notebook)
        /// </summary>
        public string NotebookId { get; set; }
    }
}
