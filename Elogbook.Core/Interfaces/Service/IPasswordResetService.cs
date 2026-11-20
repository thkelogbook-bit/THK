using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức xử lý reset password qua email OTP
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Tạo OTP và gửi email cho user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        void ForgotPassword(ForgotPasswordRequest request, string ipAddress, string userAgent);

        /// <summary>
        /// Verify OTP, trả về reset session token nếu hợp lệ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VerifyOtpData VerifyOtp(VerifyOtpRequest request);

        /// <summary>
        /// Đặt lại password mới bằng reset session token
        /// </summary>
        /// <param name="request"></param>
        void ResetPassword(ResetPasswordRequest request);

        /// <summary>
        /// Gửi lại OTP mới, invalidate OTP cũ
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        void ResendOtp(ResendOtpRequest request, string ipAddress, string userAgent);
    }
}
