namespace Elogbook.Core.Models.Response.EmailNotification
{
    /// <summary>
    /// Thông tin email hệ thống
    /// </summary>
    public class EmailConfigurationResponseModel : EntityBaseModel
    {
        /// <summary>
        /// Thư hệ thống
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Máy chủ
        /// </summary>
        public string EmailServer { get; set; }

        /// <summary>
        /// Cổng
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int EmailType { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public ValueLabelModel EmailTypeInfo { get; set; }

        /// <summary>
        /// Tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// StatusCode
        /// </summary>
        public int StatusCode { get; set; }
    }
}
