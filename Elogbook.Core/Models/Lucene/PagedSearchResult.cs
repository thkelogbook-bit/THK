using System.Collections.Generic;


namespace Elogbook.Core.Models.Lucene
{
    /// <summary>
    /// Kết quả search có phân trang.
    /// </summary>
    public class PagedSearchResult
    {
        /// <summary>
        /// Danh sách file match trong trang hiện tại.
        /// </summary>
        public List<FileSearchResult> Items { get; set; } = new List<FileSearchResult>();

        /// <summary>
        /// Tổng số kết quả match (không phân trang).
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Trang hiện tại (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Số item mỗi trang.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang.
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)System.Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Còn trang tiếp theo hay không.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Có trang trước hay không.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;
    }
}
