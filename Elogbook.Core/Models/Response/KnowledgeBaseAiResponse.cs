using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kết quả upload file lên Gemini Files API
    /// </summary>
    public class GeminiFileUploadResult
    {
        /// <summary>
        /// Tên file gốc
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// URI trả về từ Gemini Files API (dùng để gọi generateContent)
        /// </summary>
        public string FileUri { get; set; }

        /// <summary>
        /// MIME type của file
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Có lỗi trong quá trình upload không
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Thông báo lỗi nếu IsError = true
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Response model cho query AI
    /// </summary>
    public class KnowledgeBaseAiQueryResponse
    {
        /// <summary>
        /// Nội dung trả lời từ AI
        /// </summary>
        public string Answer { get; set; }
    }
}
