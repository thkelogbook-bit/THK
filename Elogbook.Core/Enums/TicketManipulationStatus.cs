using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// trạng thái phiếu thao tác
    /// </summary>
    public enum TicketManipulationStatus
    {
        /// <summary>
        /// Chưa gửi duyệt phiếu
        /// </summary>
        [Description("Chưa gửi duyệt phiếu")]
        Unsent = 1,
        /// <summary>
        /// Chờ duyệt cấp phiếu
        /// </summary>
        [Description("Chờ duyệt cấp phiếu")]
        Waiting = 2,
        /// <summary>
        /// Đã thu hồi
        /// </summary>
        [Description("Đã thu hồi")]
        Recovered = 3,
        /// <summary>
        /// Không được duyệt phiếu
        /// </summary>
        [Description("Không được duyệt phiếu")]
        Reject = 4,
        /// <summary>
        /// Đã duyệt phiếu
        /// </summary>
        [Description("Đã duyệt phiếu")]
        Approved = 5,
        /// <summary>
        /// Chưa thực hiện
        /// </summary>
        [Description("Chưa thực hiện")]
        Unfulfilled = 6,
        /// <summary>
        /// Chờ xác nhận hoàn thành
        /// </summary>
        [Description("Chờ xác nhận hoàn thành")]
        WaitForConfirmation = 7,
        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        [Description("Đã hoàn thành")]
        Completed = 8,
        /// <summary>
        /// Bị huỷ
        /// </summary>
        [Description("Bị huỷ")]
        Cancel = 9,
    }
}
