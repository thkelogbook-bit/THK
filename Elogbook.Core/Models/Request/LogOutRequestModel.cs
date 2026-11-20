namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin đăng xuất
    /// </summary>
    public class LogOutRequestModel
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tài khoản người dùng
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AccessToken { get; set; }
    }
}
