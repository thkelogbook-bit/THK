using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.EmailNotification
{
    /// <summary>
    /// Thông tin email hệ thống
    /// </summary>
    public class EmailConfigurationRequestModel
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
        /// Tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// Người dùng login ứng dụng
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
