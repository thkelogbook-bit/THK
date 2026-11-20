using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổng các hoạt động AM bất thường sự cố
    /// </summary>
    public class DashboardAbnormalTicketGroupModel
    {

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// Chưa gửi phiếu
        /// </summary>
        public int TotalNotSubmited { get; set; }
        /// <summary>
        /// đang xử lý
        /// </summary>
        public int TotalInProgress { get; set; }
        /// <summary>
        /// đã duyệt
        /// </summary>
        public int TotalTicketApproved { get; set; }

        /// <summary>
        /// dừng xử lý
        /// </summary>
        public int TotalTicketStopped { get; set; }
    }
}
