using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái truy cập của người dùng
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// Có thể truy cập
        /// </summary>
        [Description("Có thể truy cập")]
        Active = 1,
        /// <summary>
        /// Không thể truy cập
        /// </summary>
        [Description("Không thể truy cập")]
        InActive = 2
    }
}
