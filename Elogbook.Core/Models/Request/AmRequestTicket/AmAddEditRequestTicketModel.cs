using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AmAddEditRequestTicketModel
    /// </summary>
    public class AmAddEditRequestTicketModel
    {
        /// <summary>
        /// Mã mức độ khẩn cấp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public int EmergencyId { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public int? OrganizationId { get; set; }

        /// <summary>
        /// Số phiếu
        /// </summary>
        public string RequestTicketNumber { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// Chi tiết yêu cầu
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Mã số bảo trì tự quản
        /// </summary>
        public int? AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// trạng thái lưu hay đã gửi phiếu
        /// </summary>
        public bool IsSend { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
