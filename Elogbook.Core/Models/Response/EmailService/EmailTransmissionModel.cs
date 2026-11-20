namespace Elogbook.Core.Models.Response.EmailService
{
    /// <summary>
    /// EmailTransmissionModel
    /// </summary>
    public class EmailTransmissionModel : EntityBaseModel
    {
        /// <summary>
        /// Địa chỉ mail trả lời
        /// </summary>
        public string EmailReplyToAddr { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Nội dung email
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// Email html
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// Số lần thử gửi email
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// Nội dung gửi email lỗi
        /// </summary>
        public string DetailFailure { get; set; }

        /// <summary>
        /// Trạng thái gửi mail
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// Thông tin file đính kèm
        /// </summary>
        public string AttackmentID { get; set; }

        /// <summary>
        /// Flag đánh dấu đã hoàn thành việc gửi mail (hoặc số lần gửi mail vượt quá quy định)
        /// </summary>
        public bool TransmissionCompleteFlag { get; set; }

        /// <summary>
        /// mã số phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketId { get; set; }

        /// <summary>
        /// mã số phiếu công tác Điện - TCNH
        /// </summary>
        public int? WorkTicketId { get; set; }

        /// <summary>
        /// mã số phiếu bảo trì tự quản
        /// </summary>
        public int? AutonomousMaintenanceId { get; set; }

        /// <summary>
        /// mã số phiếu yêu cầu
        /// </summary>
        public int? AmRequestTicketId { get; set; }

        /// <summary>
        /// Địa chỉ mail của nhà máy
        /// </summary>
        public string FatoryEmails { get; set; }

        /// <summary>
        /// Tên của nhà máy
        /// </summary>
        public string FatoryName { get; set; }
    }
}
