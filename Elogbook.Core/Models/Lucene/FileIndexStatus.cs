using System;

namespace Elogbook.Core.Models.Lucene
{
    /// <summary>
    /// Trạng thái index của 1 file trong Lucene.
    /// </summary>
    public class FileIndexStatus
    {
        /// <summary>
        /// Đường dẫn file (normalized).
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Tên file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File đã được index hay chưa.
        /// </summary>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Thời điểm file được index lần gần nhất (UTC).
        /// Null nếu chưa index.
        /// </summary>
        public DateTime? LastIndexedUtc { get; set; }

        /// <summary>
        /// Folder chứa file (normalized).
        /// </summary>
        public string FolderPath { get; set; }
    }
}
