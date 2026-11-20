using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổng các hoạt động AM bất thường sự cố
    /// </summary>
    public class DashboardAbnormalActivityGroupModel
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
        /// Đã tạo phiếu
        /// </summary>
        public int TotalCreated { get; set; }


        /// <summary>
        /// Chua tạo phiếu
        /// </summary>
        public int TotalNotCreate { get; set; }




    }
}
