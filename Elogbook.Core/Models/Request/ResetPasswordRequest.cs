namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request cho API đặt lại password mới
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Reset session token nhận được sau khi verify OTP thành công
        /// </summary>
        public string ResetSessionToken { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        public string PasswordConfirmation { get; set; }
    }
}
