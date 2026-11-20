

namespace Elogbook.Core.Models.Lucene
{
    /// <summary>
    /// Kết quả search file từ Lucene index.
    /// </summary>
    public class FileSearchResult
    {
        /// <summary>
        /// Đường dẫn đầy đủ tới file match.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Tên file (không bao gồm thư mục).
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Điểm relevance từ Lucene (cao hơn = match tốt hơn).
        /// </summary>
        public float Score { get; set; }
    }
}

