namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmailTransmissions : EntityBase
    {
        /// <summary>
        /// địa chỉ mail trả lời
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string EmailReplyToAddr { get; set; }

        /// <summary>
        /// tiêu đề mail
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string Subject { get; set; }

        /// <summary>
        /// nội dung mail
        /// </summary>
        [Required]
        public string MessageBody { get; set; }

        /// <summary>
        /// nội dung mail có định dạng theo HTML
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// số lần đã gửi lại mail
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// nội dung lỗi trong quá trình gửi mail
        /// </summary>
        public string DetailFailure { get; set; }

        /// <summary>
        /// mail đã được gửi chưa
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// Flag đánh dấu đã hoàn thành việc gửi mail (hoặc số lần gửi mail vượt quá quy định)
        /// </summary>
        public bool TransmissionCompleteFlag { get; set; }

        /// <summary>
        /// mã số phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketId { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketId))]
        public virtual RegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// mã số phiếu công tác Điện - TCNH
        /// </summary>
        public int? WorkTicketId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketId))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// mã số phiếu bảo trì tự quản
        /// </summary>
        public int? AutonomousMaintenanceId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AutonomousMaintenanceId))]
        public virtual AutonomousMaintenance AutonomousMaintenance { get; set; }

        /// <summary>
        /// mã số phiếu yêu cầu
        /// </summary>
        public int? AmRequestTicketId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AmRequestTicketId))]
        public virtual AmRequestTicket AmRequestTicket { get; set; }

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
