using System.Collections.Generic;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin vai trò
    /// </summary>
    public class UserRolesInfo
    {
        /// <summary>
        /// Danh sách quyền
        /// </summary>
        public List<UserRightModel> UsersRight { get; set; }

        /// <summary>
        /// Giới hạn quyền truy cập
        /// </summary>
        public bool IsLimited { get; set; }

        /// <summary>
        /// Xác định vai trò Admin
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
