using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái dùng chung của AM
    /// </summary>
    public enum AmEnumStatus
    {
        /// <summary>
        /// Chưa thực hiện
        /// </summary>
        [Description("Chưa thực hiện")]
        Waiting = 1,
        /// <summary>
        /// Không thực hiện
        /// </summary>
        [Description("Không thực hiện")]
        DoNotWork,
        /// <summary>
        /// Đang thực hiện
        /// </summary>
        [Description("Đang thực hiện")]
        OnGoing,
        /// <summary>
        /// Tạm dừng
        /// </summary>
        [Description("Tạm dừng")]
        Pending,
        /// <summary>
        /// Dừng thực hiện
        /// </summary>
        [Description("Dừng thực hiện")]
        Stop,
        /// <summary>
        /// Đã thực hiện
        /// </summary>
        [Description("Đã thực hiện")]
        Completed,
        /// <summary>
        /// Tiếp tục
        /// </summary>
        [Description("Tiếp tục")]
        Continue,
    }
}
