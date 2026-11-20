namespace Elogbook.Core.Models.Lucene
{
    /// <summary>
    /// Kết quả debug extract text từ file — dùng để kiểm tra iTextSharp/NPOI
    /// có đọc được nội dung file không trước khi index vào Lucene.
    /// </summary>
    public class ExtractTextDebugResult
    {
        /// <summary>Đường dẫn file đã kiểm tra.</summary>
        public string FilePath { get; set; }

        /// <summary>File có tồn tại trên disk không.</summary>
        public bool FileExists { get; set; }

        /// <summary>Extension của file.</summary>
        public string Extension { get; set; }

        /// <summary>Extract có thành công không.</summary>
        public bool ExtractSuccess { get; set; }

        /// <summary>Độ dài tổng của text extract được (số ký tự).</summary>
        public int ExtractedLength { get; set; }

        /// <summary>500 ký tự đầu của text extract được — để xác nhận nội dung.</summary>
        public string PreviewText { get; set; }

        /// <summary>Thông báo lỗi nếu extract thất bại.</summary>
        public string ErrorMessage { get; set; }
    }
}
