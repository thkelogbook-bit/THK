using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức business cho chức năng reset password qua email OTP
    /// </summary>
    public interface IPasswordResetBusiness
    {
        /// <summary>
        /// Xử lý yêu cầu gửi OTP reset password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        ResponseModel<bool> ForgotPassword(ForgotPasswordRequest request, string ipAddress, string userAgent);

        /// <summary>
        /// Verify OTP, trả về reset session token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponseModel<VerifyOtpData> VerifyOtp(VerifyOtpRequest request);

        /// <summary>
        /// Đặt lại password mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponseModel<bool> ResetPassword(ResetPasswordRequest request);

        /// <summary>
        /// Gửi lại OTP mới
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        ResponseModel<bool> ResendOtp(ResendOtpRequest request, string ipAddress, string userAgent);
    }
}
