using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    ///Ttrạng thái phiếu công tác
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// Chưa gửi phiếu đăng ký
        /// </summary>
        [Description("Chưa gửi phiếu đăng ký")]
        Unsent = 1,
        /// <summary>
        /// Chờ cấp phiếu công tác
        /// </summary>
        [Description("Chờ cấp phiếu công tác")]
        Waiting,
        /// <summary>
        /// Đã thu hồi
        /// </summary>
        [Description("Đã thu hồi")]
        Cancel,
        /// <summary>
        /// Không được cấp phiếu
        /// </summary>
        [Description("Không được cấp phiếu")]
        Reject,
        /// <summary>
        /// Đã cấp phiếu công tác
        /// </summary>
        [Description("Đã cấp phiếu công tác")]
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
