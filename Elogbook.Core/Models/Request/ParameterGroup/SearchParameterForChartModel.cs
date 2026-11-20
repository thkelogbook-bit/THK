using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tìm kiếm thông số vẽ biểu đồ trend
    /// </summary>
    public class SearchParameterForChartModel
    {
        /// <summary>
        /// Đánh dấu khi tìm kiểm
        /// </summary>
        public bool IsSearch { get; set; }

        /// <summary>
        /// Mã thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
