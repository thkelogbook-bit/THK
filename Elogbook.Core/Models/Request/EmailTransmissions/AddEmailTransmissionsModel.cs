namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AddEmailTransmissionsModel
    /// </summary>
    public class AddEmailTransmissionsModel
    {
        /// <summary>
        /// địa chỉ mail trả lời
        /// </summary>
        public string EmailReplyToAddr { get; set; }

        /// <summary>
        /// tiêu đề mail
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// nội dung mail
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// nội dung mail có định dạng theo HTML
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// số lần đã gửi lại mail
        /// </summary>
        public int Retry { get; set; } = 0;

        /// <summary>
        /// nội dung lỗi trong quá trình gửi mail
        /// </summary>
        public string DetailFailure { get; set; }

        /// <summary>
        /// mã số của file đính kèm
        /// </summary>
        public string AttackmentID { get; set; }

        /// <summary>
        /// mail đã được gửi chưa
        /// </summary>
        public bool IsSent { get; set; } = false;

        /// <summary>
        /// cờ đánh dấu đã hoàn thành việc gửi mail (hoặc số lần gửi mail vượt quá quy định)
        /// </summary>
        public bool TransmissionCompleteFlag { get; set; }

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
