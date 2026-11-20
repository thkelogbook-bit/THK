using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response.Dashboard
{
    /// <summary>
    /// Thống kê trình trạng giao nhận ca trong ngày
    /// </summary>
    public class DashboardPositionShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Tổng số chưa giao/ nhân ca
        /// </summary>
        public int TotalPendingShiftHandovers { get; set; } = 0;

        /// <summary>
        /// Tổng số đã giao ca
        /// </summary>
        public int TotalShiftHandovers { get; set; } = 0;

        /// <summary>
        /// tổng số ca
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
    /// </summary>
    public class DashboardPlantShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Tổng số chưa xử lý
        /// </summary>
        public int TotalPendingCount { get; set; } = 0;

        /// <summary>
        /// Tổng số đang xử lý
        /// </summary>
        public int TotalProcessingCount { get; set; } = 0;

        /// <summary>
        /// Tổng số đã xử lý
        /// </summary>
        public int TotalProcessedCount { get; set; } = 0;
    }
}
