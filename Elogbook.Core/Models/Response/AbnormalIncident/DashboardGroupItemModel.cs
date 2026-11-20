using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin báo cáo xử lý bất thường sự cố
    /// </summary>
    public class DashboardGroupItemModel
    {
        /// <summary>
        /// Nha may
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Dữ liệu chi tiết của 1 tháng
        /// </summary>
        public List<DataDetailSingelMonth> DataDetail { get; set; }

        /// <summary>
        /// Tổng bất thường của 1 nhà máy
        /// </summary>
        public int TotalAbnormalSingelFactory { get; set; } = 0;
    }

    /// <summary>
    /// Dữ liệu chi tiết của 1 tháng
    /// </summary>
    public class DataDetailSingelMonth
    {
        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Tổng hoạt động
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// Tổng bất thường
        /// </summary>
        public int TotalAbnormal { get; set; } = 0;

        /// <summary>
        /// Tổng PYC đã tạo
        /// </summary>
        public int TotalOpenTicket { get; set; } = 0;

        /// <summary>
        ///Tổng PYC chưa tạo
        /// </summary>
        public int TotalNotOpen { get; set; } = 0;
    }
}
