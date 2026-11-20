namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request cho API yêu cầu gửi OTP reset password
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Email người dùng
        /// </summary>
        public string Email { get; set; }
    }
}
