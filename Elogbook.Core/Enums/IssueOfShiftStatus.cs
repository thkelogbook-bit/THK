using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái tồn tại trong ca
    /// </summary>
    public enum IssueOfShiftStatus
    {
        /// <summary>
        /// Chưa xử lý
        /// </summary>
        [Description("Chưa xử lý")]
        NOT_PROCESS = 0,
        /// <summary>
        /// Đang xử lý
        /// </summary>
        [Description("Đang xử lý")]
        IN_PROGRESS = 1,
        /// <summary>
        /// Đã xử lý
        /// </summary>
        [Description("Đã xử lý")]
        DONE = 2,
        /// <summary>
        /// Xóa
        /// </summary>
        [Description("Xóa")]
        DELETE = 3,
    }
}
