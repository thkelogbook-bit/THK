using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổng các hoạt động AM
    /// </summary>
    public class DashboardAllActivityGroupModel
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
        /// Hoàn thanh
        /// </summary>
        public int TotalCompleted { get; set; }

        /// <summary>
        /// Khoong thuc hien
        /// </summary>
        public int TotalUnProcess { get; set; }

        /// <summary>
        /// Chua thuc hien
        /// </summary>
        public int TotalNotYetProcess { get; set; }

        /// <summary>
        /// Đang thuc hien
        /// </summary>
        public int TotalOnGoing { get; set; }




    }
}
