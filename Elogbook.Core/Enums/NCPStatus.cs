using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái thay đổi người cho phép
    /// </summary>
    public enum NCPStatus
    {
        /// <summary>
        /// Chờ duyệt đổi người cho phép
        /// </summary>
        [Description("Chờ duyệt đổi người cho phép")]
        WAITING = 1,
        /// <summary>
        /// Đã duyệt đổi người cho phép
        /// </summary>
        [Description("Đã duyệt đổi người cho phép")]
        APPROVED,
        /// <summary>
        /// Không duyệt đổi người cho phép
        /// </summary>
        [Description("Không duyệt đổi người cho phép")]
        REJECT
    }
}
