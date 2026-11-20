namespace Elogbook.Core.Models.Lucene
{
    /// <summary>Request cho debug endpoint IndexAndSearch.</summary>
    public class IndexAndSearchDebugRequest
    {
        public string FilePath { get; set; }
        public string Query { get; set; }
    }

    /// <summary>
    /// Kết quả debug pipeline index → search end-to-end.
    /// Hiển thị từng bước để xác định bước nào bị lỗi.
    /// </summary>
    public class IndexAndSearchDebugResult
    {
        /// <summary>Path gốc truyền vào.</summary>
        public string RawFilePath { get; set; }

        /// <summary>Path sau khi NormalizePath() — đây là key lưu trong index.</summary>
        public string NormalizedFilePath { get; set; }

        /// <summary>Query gốc truyền vào.</summary>
        public string RawQuery { get; set; }

        /// <summary>IndexFile đã chạy thành công chưa.</summary>
        public bool IndexSuccess { get; set; }

        /// <summary>indexStatus sau khi index: IsIndexed có = true không.</summary>
        public bool IsIndexedAfterIndex { get; set; }

        /// <summary>Path lưu trong index (lấy từ indexStatus) — so sánh với NormalizedFilePath.</summary>
        public string PathStoredInIndex { get; set; }

        /// <summary>Kết quả SearchInFile sau khi index.</summary>
        public bool SearchResult { get; set; }

        /// <summary>Thông báo lỗi nếu có.</summary>
        public string ErrorMessage { get; set; }
    }
}
