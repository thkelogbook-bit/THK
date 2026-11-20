namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa các kiểu dữ liệu chung của hệ thống
    /// </summary>
    public enum SystemDataType
    {
        /// <summary>
        /// Các events gửi mail
        /// </summary>
        EmailNotificationEvent = 1,
        /// <summary>
        /// Các chức năng có sự kiện gửi email
        /// </summary>
        ModuleName = 2,
        /// <summary>
        /// Chức năng hoạt động trên mobile
        /// </summary>
        MobileActivity = 3,
        /// <summary>
        /// Danh mục trạng thái
        /// </summary>
        StatusCategory = 4,
        /// <summary>
        /// Sự kiện đồng bộ dữ liệu phiếu công tác điện sang theo dõi tồn tại ca
        /// </summary>
        SyncPCTD = 5,
        /// <summary>
        /// Sự kiện đồng bộ dữ liệu phiếu công tác thủy-cơ-nhiệt-hóa sang theo dõi tồn tại ca
        /// </summary>
        SyncPCTTCNH = 6,
        /// <summary>
        /// Nhóm người dùng để gửi email thông báo và notification
        /// </summary>
        UserEmailGroup = 7,
    }
}
