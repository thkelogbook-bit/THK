using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Authentication — Reset password qua email OTP
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/auth")]
    [AllowAnonymous]
    public class AuthController : ApiController
    {
        private readonly IPasswordResetBusiness _passwordResetBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthController(IPasswordResetBusiness passwordResetBusiness)
        {
            _passwordResetBusiness = passwordResetBusiness;
        }

        /// <summary>
        /// Yêu cầu gửi mã OTP reset password qua email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("forgot-password")]
        [HttpPost]
        public ResponseModel<bool> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();
            return _passwordResetBusiness.ForgotPassword(request, ipAddress, userAgent);
        }

        /// <summary>
        /// Xác thực OTP, nhận reset session token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("verify-otp")]
        [HttpPost]
        public ResponseModel<VerifyOtpData> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            return _passwordResetBusiness.VerifyOtp(request);
        }

        /// <summary>
        /// Đặt lại mật khẩu mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("reset-password")]
        [HttpPost]
        public ResponseModel<bool> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            return _passwordResetBusiness.ResetPassword(request);
        }

        /// <summary>
        /// Gửi lại mã OTP mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("resend-otp")]
        [HttpPost]
        public ResponseModel<bool> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();
            return _passwordResetBusiness.ResendOtp(request, ipAddress, userAgent);
        }

        #region Helpers

        private string GetClientIpAddress()
        {
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
                return ctx?.Request?.UserHostAddress;
            }
            return null;
        }

        private string GetUserAgent()
        {
            if (Request.Headers.UserAgent != null)
                return Request.Headers.UserAgent.ToString();
            return null;
        }

        #endregion
    }
}
