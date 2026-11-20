using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    ///Ttrạng thái lệnh công tác
    /// </summary>
    public enum WorkPermitStatus
    {
        /// <summary>
        /// Chưa gửi lệnh đăng ký
        /// </summary>
        [Description("Chưa gửi lệnh đăng ký")]
        Unsent = 1,
        /// <summary>
        /// Chờ cấp lệnh công tác
        /// </summary>
        [Description("Chờ cấp lệnh công tác")]
        Waiting,
        /// <summary>
        /// Đã thu hồi
        /// </summary>
        [Description("Đã thu hồi")]
        Cancel,
        /// <summary>
        /// Không được cấp lệnh
        /// </summary>
        [Description("Không được cấp lệnh")]
        Reject,
        /// <summary>
        /// Đã cấp lệnh công tác
        /// </summary>
        [Description("Đã cấp lệnh công tác")]
        Approved,
        /// <summary>
        /// Đã cho phép
        /// </summary>
        [Description("Đã cho phép")]
        Allowed,
        /// <summary>
        /// Đang thực hiện công tác
        /// </summary>
        [Description("Đang thực hiện")]
        InProgress,
        /// <summary>
        /// Chờ xác nhận hoàn thành
        /// </summary>
        [Description("Chờ xác nhận hoàn thành")]
        WaitForConfirmation,
        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        [Description("Đã hoàn thành")]
        Completed,
    }
}
