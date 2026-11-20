using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phiếu yêu cầu
    /// </summary>
    public class AmRequestTicketModel : EntityBaseModel
    {
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
        /// Nhà máy
        /// </summary>
        public ComboboxDataModel Factory { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public AmStatusInfoModel Status { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public ComboboxDataModel AmLocation { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ComboboxDataModel AmDevice { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public ComboboxDataModel AmEmergency { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        [JsonIgnore]
        public int AmEmergencyID { get; set; }

        /// <summary>
        /// Đơn vị công tác
        /// </summary>
        public ComboboxDataModel Organization { get; set; }

        /// <summary>
        /// Dánh sách ID file đính kèm
        /// </summary>
        public string AttachmentIds { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<AttachmentModel> Attachments { get; set; }

        /// <summary>
        /// Số phiếu đăng ký
        /// </summary>
        public string RegistrationName { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Loại phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketType { get; set; }

        /// <summary>
        /// ngày bắt đầu gửi phiếu
        /// </summary>
        public DateTime? InsertedAtBySend { get; set; }

        /// <summary>
        /// Mã số bảo trì tự quản
        /// </summary>
        public int? AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Thứ tự trạng thái
        /// </summary>
        public int PositionStatus { get; set; }
    }
}
