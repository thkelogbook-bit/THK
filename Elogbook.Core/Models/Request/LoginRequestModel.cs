namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin yêu cầu đăng nhập
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// Tài khoản người dùng
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Nếu login từ Mobile app thì truyền true/nếu login từ web app truyền false
        /// </summary>
        public bool IsMobile { get; set; }
    }
}
