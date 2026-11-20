namespace Elogbook.Core.Models.Response.EmailService
{
    /// <summary>
    /// EmailTransmissionRecipientsModel
    /// </summary>
    public class EmailTransmissionRecipientsModel : EntityBaseModel
    {
        /// <summary>
        /// mã số của Email Transmission
        /// </summary>
        public int EmailTransmissionID { get; set; }

        /// <summary>
        /// địa chỉ mail nhận
        /// </summary>
        public string RecipientEmailAddr { get; set; }

        /// <summary>
        /// tên mail
        /// </summary>
        public string RecipientEmailName { get; set; }

        /// <summary>
        /// loại mail: 1:CC,2:BCC,3:To
        /// </summary>
        public int RecipientType { get; set; }
    }
}
