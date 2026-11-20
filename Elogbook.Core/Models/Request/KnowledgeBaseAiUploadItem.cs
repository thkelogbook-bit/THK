using System.IO;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin một file cần upload lên Gemini Files API
    /// </summary>
    public class KnowledgeBaseAiUploadItem
    {
        /// <summary>Tên file gốc</summary>
        public string FileName { get; set; }

        /// <summary>Nội dung file</summary>
        public Stream Content { get; set; }

        /// <summary>MIME type, ví dụ application/pdf</summary>
        public string MimeType { get; set; }
    }
}
