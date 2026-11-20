using System.Collections.Generic;

namespace Elogbook.Core.Models.Lucene
{
    /// <summary>Kết quả liệt kê toàn bộ file đã index trong Lucene.</summary>
    public class IndexedFileListResult
    {
        /// <summary>Tổng số document trong index.</summary>
        public int TotalCount { get; set; }

        /// <summary>Danh sách các file đã index.</summary>
        public List<IndexedFileInfo> Files { get; set; } = new List<IndexedFileInfo>();
    }

    public class IndexedFileInfo
    {
        /// <summary>Path đã normalize — đây là key lưu trong index.</summary>
        public string FilePath { get; set; }

        /// <summary>Tên file.</summary>
        public string FileName { get; set; }

        /// <summary>Folder chứa file.</summary>
        public string FolderPath { get; set; }

        /// <summary>Thời điểm index gần nhất (UTC).</summary>
        public string LastIndexedUtc { get; set; }
    }
}
