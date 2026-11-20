using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Khai báo danh sách các chức năng gửi thông báo
    /// </summary>
    public enum EnumModuleNotification
    {
        /// <summary>
        /// Phiếu công tác Điện
        /// </summary>
        [Description("Phiếu công tác điện")]
        PCTD = 1,
        /// <summary>
        /// Phiếu công tác TCNH
        /// </summary>
        [Description("Phiếu công tác thủy-cơ-nhiệt-hóa")]
        TCNH,
        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        [Description("Bảo trì tự quản")]
        AM,
        /// <summary>
        /// Phiếu yêu cầu
        /// </summary>
        [Description("Phiếu yêu cầu")]
        PYC
    }

    /// <summary>
    /// khai báo enum module cho notification
    /// </summary>
    public enum EnumNotificationType
    {
        /// <summary>
        /// Đăng ký
        /// </summary>
        [Description("Tạo phiếu đăng ký công tác/Phiếu thao tác/Phiếu yêu cầu")]
        Register = 1,
        /// <summary>
        /// Thu hồi
        /// </summary>
        [Description("Thu hồi phiếu đăng ký công tác/Phiếu thao tác")]
        Cancel,
        /// <summary>
        /// Từ chối
        /// </summary>
        [Description("Từ chối phiếu đăng ký công tác/Phiếu thao tác/Phiếu yêu cầu")]
        Reject,
        /// <summary>
        /// Cấp phiếu
        /// </summary>
        [Description("Cấp phiếu đăng ký công tác/Phiếu thao tác")]
        Approver,
        /// <summary>
        /// Cấp phiếu (tự tạo mới)
        /// </summary>
        [Description("Cấp phiếu công tác (tự thêm mới)")]
        ApproverByAdd,
        /// <summary>
        /// Cho phép
        /// </summary>
        [Description("Cho phép thực hiện phiếu công tác/Phiếu thao tác")]
        Allow,
        /// <summary>
        /// Hoàn thành
        /// </summary>
        [Description("Thực hiện hoàn thành phiếu công tác/Phiếu thao tác")]
        Complete,
        /// <summary>
        /// nhắc nhở lịch
        /// </summary>
        [Description("Nhắc lịch thực hiện hoạt động AM")]
        AmCalendarReminder,
        /// <summary>
        /// Thông báo bất thường, thiết bị
        /// </summary>
        [Description("Thông báo bất thường, thiết bị")]
        AmNotificationDevice,
        /// <summary>
        /// Báo cáo, thống kê
        /// </summary>
        [Description("Báo cáo, thống kê hoạt động AM")]
        AmStatistic,
        ///// <summary>
        ///// Báo cáo, thống kê
        ///// </summary>
        //[Description("Có phiếu yêu cầu cần phê duyệt")]
        //PycRegister,
        ///// <summary>
        ///// Báo cáo, thống kê
        ///// </summary>
        //[Description("Từ chối phiếu yêu cầu")]
        //PycReject,
        /// <summary>
        /// Phê duyệt phiếu yêu cầu - Gửi nội bộ
        /// </summary>
        [Description("Phê duyệt phiếu yêu cầu - Gửi nội bộ")]
        PycApprovedTC,
        /// <summary>
        /// Phê duyệt phiếu yêu cầu - Gửi đơn vị sửa chữa
        /// </summary>
        [Description("Phê duyệt phiếu yêu cầu - Gửi đơn vị sửa chữa")]
        PycApprovedVHV,
        /// <summary>
        /// Dừng xử lý phiếu yêu cầu
        /// </summary>
        [Description("Dừng xử lý phiếu yêu cầu")]
        PycStopTC,
        /// <summary>
        /// Gửi thông báo trước hạn so với thời gian kết thúc PCT
        /// </summary>
        [Description("Gửi thông báo trước hạn so với thời gian kết thúc PCT")]
        BeforeDueCompletion,
        /// <summary>
        /// Gửi thông báo vào ngày dự kiến hoàn thành PCT
        /// </summary>
        [Description("Gửi thông báo đến hạn hoàn thành PCT")]
        DueTodayCompletion,
        /// <summary>
        /// Gửi thông báo quá hạn so với thời gian kết thúc PCT
        /// </summary>
        [Description("Gửi thông báo quá hạn so với thời gian kết thúc PCT")]
        OverdueCompletion
    }

    /// <summary>
    /// Các sự kiện gửi Email
    /// </summary>
    public enum NotificationEvent
    {
        /// <summary>
        /// Có phiếu yêu cầu cần phê duyệt
        /// </summary>
        PYC_APPROVED = 18,
    }
}
