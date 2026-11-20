using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Elogbook.Business
{
    /// <summary>
    /// Business layer cho chức năng reset password qua email OTP
    /// </summary>
    public class PasswordResetBusiness : IPasswordResetBusiness
    {
        private readonly IPasswordResetService _passwordResetService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PasswordResetBusiness(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        /// <inheritdoc/>
        public ResponseModel<bool> ForgotPassword(ForgotPasswordRequest request, string ipAddress, string userAgent)
        {
            var response = new ResponseModel<bool>();

            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email là thông tin bắt buộc.");
                    return response;
                }

                if (!IsValidEmail(request.Email))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email không đúng định dạng.");
                    return response;
                }

                _passwordResetService.ForgotPassword(request, ipAddress, userAgent);

                // Luôn trả về thành công dù email có tồn tại hay không (tránh user enumeration)
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra, vui lòng thử lại sau.");
                LogCustom.LogMessage(Core.Enums.TrackingLogLevel.ERROR, nameof(PasswordResetBusiness),
                    Environment.NewLine + $"ForgotPassword - Email: {request?.Email}" + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <inheritdoc/>
        public ResponseModel<VerifyOtpData> VerifyOtp(VerifyOtpRequest request)
        {
            var response = new ResponseModel<VerifyOtpData>();

            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email là thông tin bắt buộc.");
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.Otp))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Mã OTP là thông tin bắt buộc.");
                    return response;
                }

                if (!Regex.IsMatch(request.Otp.Trim(), @"^\d{6}$"))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Mã OTP phải là 6 chữ số.");
                    return response;
                }

                response.Data = _passwordResetService.VerifyOtp(request);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(Core.Enums.TrackingLogLevel.ERROR, nameof(PasswordResetBusiness),
                    Environment.NewLine + $"VerifyOtp - Email: {request?.Email}" + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <inheritdoc/>
        public ResponseModel<bool> ResetPassword(ResetPasswordRequest request)
        {
            var response = new ResponseModel<bool>();

            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.ResetSessionToken))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Reset session token là thông tin bắt buộc.");
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Mật khẩu mới là thông tin bắt buộc.");
                    return response;
                }

                if (request.Password.Length < 8)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Mật khẩu phải có ít nhất 8 ký tự.");
                    return response;
                }

                if (!string.Equals(request.Password, request.PasswordConfirmation))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Mật khẩu xác nhận không khớp.");
                    return response;
                }

                _passwordResetService.ResetPassword(request);
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(Core.Enums.TrackingLogLevel.ERROR, nameof(PasswordResetBusiness),
                    Environment.NewLine + "ResetPassword" + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <inheritdoc/>
        public ResponseModel<bool> ResendOtp(ResendOtpRequest request, string ipAddress, string userAgent)
        {
            var response = new ResponseModel<bool>();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email là thông tin bắt buộc.");
                    return response;
                }

                if (!IsValidEmail(request.Email))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email không đúng định dạng.");
                    return response;
                }

                _passwordResetService.ResendOtp(request, ipAddress, userAgent);
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra, vui lòng thử lại sau.");
                LogCustom.LogMessage(Core.Enums.TrackingLogLevel.ERROR, nameof(PasswordResetBusiness),
                    Environment.NewLine + $"ResendOtp - Email: {request?.Email}" + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        #region Private helpers

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }

        #endregion
    }
}
