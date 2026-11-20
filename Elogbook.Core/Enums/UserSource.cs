using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Nguồn của người dùng
    /// </summary>
    public enum UserSource
    {
        /// <summary>
        /// Người dùng AD
        /// </summary>
        [Description("AD")]
        ADUser = 1,
        /// <summary>
        /// Người dùng được tạo từ Admin Portal
        /// </summary>
        [Description("Admin Portal")]
        AdminPortal,
    }
}
