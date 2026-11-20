using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổng các hoạt động AM bất thường sự cố
    /// </summary>
    public class DashboardAMUserGroupModel
    {
        /// <summary>
        /// Thông tin người dùng
        /// </summary>
        public DashboardUserModel UserInfo { get; set; }

        /// <summary>
        /// Tổng Am
        /// </summary>
        public int TotalAm { get; set; }
    }
}
