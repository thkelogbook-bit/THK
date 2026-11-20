namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request cho API gửi lại OTP
    /// </summary>
    public class ResendOtpRequest
    {
        /// <summary>
        /// Email người dùng
        /// </summary>
        public string Email { get; set; }
    }
}
