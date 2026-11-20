namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Response data cho API verify OTP thành công
    /// </summary>
    public class VerifyOtpData
    {
        /// <summary>
        /// Reset session token tạm thời (5 phút)
        /// </summary>
        public string ResetSessionToken { get; set; }

        /// <summary>
        /// Số giây còn lại trước khi token hết hạn
        /// </summary>
        public int ExpiresInSeconds { get; set; }
    }
}
