using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    ///Ttrạng thái phiếu công tác
    /// </summary>
    public enum EnumTicketStatus
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
        Waiting = 2,
        /// <summary>
        /// Đã thu hồi
        /// </summary>
        [Description("Đã thu hồi")]
        Cancel = 3,
        /// <summary>
        /// Không được cấp phiếu
        /// </summary>
        [Description("Không được cấp phiếu")]
        Reject = 4,
        /// <summary>
        /// Đã cấp phiếu công tác
        /// </summary>
        [Description("Đã cấp phiếu công tác")]
        Approved = 5,
        /// <summary>
        /// Đã cho phép
        /// </summary>
        [Description("Đã cho phép")]
        Allowed = 6,
        /// <summary>
        /// Đang thực hiện công tác
        /// </summary>
        [Description("Đang thực hiện")]
        InProgress = 7,
        /// <summary>
        /// Chờ xác nhận hoàn thành
        /// </summary>
        [Description("Chờ xác nhận hoàn thành")]
        WaitForConfirmation = 8,
        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        [Description("Đã hoàn thành")]
        Completed = 9,
        /// <summary>
        /// Chờ thay đổi người cho phép
        /// </summary>
        [Description("Chờ thay đổi người cho phép")]
        WaitForChangeCommander = 10,
        /// <summary>
        /// Chờ đăng ký
        /// </summary>
        [Description("Chờ đăng ký")]
        WaitForRegist = 11,
        /// <summary>
        /// Không cho phép thực hiện
        /// </summary>
        [Description("Không cho phép thực hiện")]
        NotAllowed = 12,
        /// <summary>
        /// Đã hủy
        /// </summary>
        [Description("Đã Hủy")]
        Annulled = 13,
        /// <summary>
        /// Chờ vật tư
        /// </summary>
        [Description("Chờ vật tư")]
        WaitingMaterials = 14,
        /// <summary>
        /// Không thực hiện
        /// </summary>
        [Description("Không thực hiện")]
        Skipped = 15,
        /// <summary>
        /// Không thực hiện
        /// </summary>
        [Description("Khóa hết hạn")]
        ExpiredLock = 16
    }
}
