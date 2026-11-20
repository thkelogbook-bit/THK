using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại nguyên nhân
    /// </summary>
    public enum EnumNotification
    {
        /// <summary>
        /// Đã đọc
        /// </summary>
        [Description("Đã đọc")]
        Read = 1,

        /// <summary>
        /// Chưa đọc
        /// </summary>
        [Description("Chưa đọc")]
        Unread
    }
}
