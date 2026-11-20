using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại tài khoản người dùng
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Hai hệ thống
        /// </summary>
        [Description("Hai hệ thống")]
        TwoSystem = 1,
        /// <summary>
        /// AdminPortal
        /// </summary>
        [Description("Admin Portal")]
        AdminPortal,
        /// <summary>
        /// MobileApp
        /// </summary>
        [Description("Mobile App")]
        MobileApp,
    }
}
