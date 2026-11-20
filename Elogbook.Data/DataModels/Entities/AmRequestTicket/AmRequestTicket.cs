namespace Elogbook.Data
{
    using Elogbook.Core.Enums;
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AmRequestTicket")]
    public class AmRequestTicket : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public int? AmLocationId { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmLocationId))]
        public virtual AmLocation AmLocation { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int? AmDeviceId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceId))]
        public virtual AmDevice AmDevice { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public int? AmEmergencyId { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmEmergencyId))]
        public virtual AmEmergency AmEmergency { get; set; }

        /// <summary>
        /// Đơn vị công tác
        /// </summary>
        public int? OrganizationId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }

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
        /// Trạng thái
        /// </summary>
        [Required]
        public int AmStatusID { get; set; } = (int)AmEnumRequestTicket.InProgress;

        [MapperIgnore]
        [ForeignKey(nameof(AmStatusID))]
        public virtual AmRequestTicketStatus RequestTicketStatus { get; set; }

        /// <summary>
        /// Mã số bảo trì tự quản
        /// </summary>
        public int? AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Loại phiếu đăng ký công tác (1: PCT Điện, 2 : PCT TCNH)
        /// </summary>
        public int? RegistrationTicketType { get; set; }

        /// <summary>
        /// ngày bắt đầu gửi phiếu
        /// </summary>
        public DateTime? InsertedAtBySend { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
