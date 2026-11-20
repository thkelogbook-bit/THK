namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AddEmailTransmissionRecipientsModel
    /// </summary>
    public class AddEmailTransmissionRecipientsModel
    {
        /// <summary>
        /// mã số của Email Transmission
        /// </summary>
        public int EmailTransmissionID { get; set; }

        /// <summary>
        /// địa chỉ mail người nhận
        /// </summary>
        public string RecipientEmailAddr { get; set; }

        /// <summary>
        /// họ và tên người nhận
        /// </summary>
        public string RecipientEmailName { get; set; }

        /// <summary>
        /// UserID Người nhận
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// loại mail: 1:CC,2:BCC,3:To
        /// </summary>
        public int RecipientType { get; set; }
    }
}
