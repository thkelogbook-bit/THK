namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Request cho API verify OTP
    /// </summary>
    public class VerifyOtpRequest
    {
        /// <summary>
        /// Email người dùng
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Mã OTP 6 chữ số
        /// </summary>
        public string Otp { get; set; }
    }
}
