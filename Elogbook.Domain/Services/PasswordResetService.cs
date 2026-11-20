using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Data;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý logic reset password qua email OTP
    /// </summary>
    public class PasswordResetService : IPasswordResetService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IEmailConfigurationService _emailConfigurationService;

        // OTP hết hạn sau 10 phút
        private const int OTP_EXPIRES_MINUTES = 10;
        // Reset session token hết hạn sau 5 phút
        private const int RESET_TOKEN_EXPIRES_MINUTES = 5;
        // Số lần thử OTP tối đa
        private const int MAX_OTP_ATTEMPTS = 5;

        /// <summary>
        /// Constructor
        /// </summary>
        public PasswordResetService(ElogbookUnitOfWork unitOfWork, IEmailConfigurationService emailConfigurationService)
        {
            _unitOfWork = unitOfWork;
            _emailConfigurationService = emailConfigurationService;
        }

        /// <inheritdoc/>
        public void ForgotPassword(ForgotPasswordRequest request, string ipAddress, string userAgent)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
                return;

            // Tìm user theo email — nếu không tìm thấy vẫn trả về thành công (tránh user enumeration)
            var user = _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => x.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase) && !x.IsDeleted)
                .FirstOrDefault();

            if (user == null)
                return;

            // Invalidate toàn bộ OTP cũ chưa dùng của user này
            var oldOtps = _unitOfWork.ElogbookRepository<PasswordResetOtp>()
                .GetQuery(x => x.UserID == user.ID && !x.Used && !x.IsDeleted)
                .ToList();
            foreach (var old in oldOtps)
            {
                old.Used = true;
                _unitOfWork.ElogbookRepository<PasswordResetOtp>().Update(old);
            }

            // Tạo OTP 6 số cryptographically secure
            var otpPlain = GenerateOtp();
            var otpHash = ComputeSha256(otpPlain);

            // Lưu vào DB
            var otpEntity = new PasswordResetOtp
            {
                UserID = user.ID,
                OtpHash = otpHash,
                ExpiresAt = DateTime.Now.AddMinutes(OTP_EXPIRES_MINUTES),
                Used = false,
                Attempts = 0,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedUserID = user.ID,
                LastUpdatedUserID = user.ID,
            };
            _unitOfWork.ElogbookRepository<PasswordResetOtp>().Add(otpEntity);
            _unitOfWork.Complete();

            // Gửi email chứa OTP
            SendOtpEmail(user.Email, user.FullName, otpPlain);
        }

        /// <inheritdoc/>
        public VerifyOtpData VerifyOtp(VerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Otp))
                throw new Exception("Email hoặc OTP không hợp lệ.");

            // Tìm user
            var user = _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => x.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase) && !x.IsDeleted)
                .FirstOrDefault();

            if (user == null)
                throw new Exception("Mã OTP không đúng hoặc đã hết hạn.");

            // Hash OTP từ request
            var otpHash = ComputeSha256(request.Otp.Trim());

            // Tìm OTP record hợp lệ — chưa dùng, chưa hết hạn, chưa quá attempts
            var otpRecord = _unitOfWork.ElogbookRepository<PasswordResetOtp>()
                .GetQuery(x => x.UserID == user.ID
                            && !x.Used
                            && !x.IsDeleted
                            && x.ExpiresAt > DateTime.Now
                            && x.Attempts < MAX_OTP_ATTEMPTS)
                .OrderByDescending(x => x.InsertedAt)
                .FirstOrDefault();

            if (otpRecord == null)
                throw new Exception("Mã OTP đã hết hạn hoặc không hợp lệ. Vui lòng yêu cầu mã mới.");

            // Lấy entity để update (cần tracked entity)
            var otpEntity = _unitOfWork.ElogbookRepository<PasswordResetOtp>().GetById(otpRecord.ID);

            // Kiểm tra hash có khớp không
            if (!string.Equals(otpEntity.OtpHash, otpHash, StringComparison.OrdinalIgnoreCase))
            {
                // Tăng attempts
                otpEntity.Attempts = (short)(otpEntity.Attempts + 1);
                otpEntity.UpdatedAt = DateTime.Now;
                _unitOfWork.ElogbookRepository<PasswordResetOtp>().Update(otpEntity);
                _unitOfWork.Complete();

                int remaining = MAX_OTP_ATTEMPTS - otpEntity.Attempts;
                if (remaining <= 0)
                    throw new Exception("Quá số lần thử cho phép. Vui lòng yêu cầu mã mới.");

                throw new Exception($"Mã OTP không đúng. Bạn còn {remaining} lần thử.");
            }

            // OTP hợp lệ — đánh dấu đã dùng
            otpEntity.Used = true;
            otpEntity.UpdatedAt = DateTime.Now;
            _unitOfWork.ElogbookRepository<PasswordResetOtp>().Update(otpEntity);

            // Tạo reset session token
            var tokenPlain = GenerateSecureToken();
            var tokenHash = ComputeSha256(tokenPlain);

            var tokenEntity = new ResetSessionToken
            {
                UserID = user.ID,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.Now.AddMinutes(RESET_TOKEN_EXPIRES_MINUTES),
                Used = false,
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedUserID = user.ID,
                LastUpdatedUserID = user.ID,
            };
            _unitOfWork.ElogbookRepository<ResetSessionToken>().Add(tokenEntity);
            _unitOfWork.Complete();

            return new VerifyOtpData
            {
                ResetSessionToken = tokenPlain,
                ExpiresInSeconds = RESET_TOKEN_EXPIRES_MINUTES * 60,
            };
        }

        /// <inheritdoc/>
        public void ResetPassword(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ResetSessionToken))
                throw new Exception("Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Mật khẩu mới không được để trống.");

            if (!string.Equals(request.Password, request.PasswordConfirmation))
                throw new Exception("Mật khẩu xác nhận không khớp.");

            // Tìm token
            var tokenHash = ComputeSha256(request.ResetSessionToken.Trim());

            var tokenRecord = _unitOfWork.ElogbookRepository<ResetSessionToken>()
                .GetQuery(x => x.TokenHash == tokenHash
                            && !x.Used
                            && !x.IsDeleted
                            && x.ExpiresAt > DateTime.Now)
                .FirstOrDefault();

            if (tokenRecord == null)
                throw new Exception("Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

            // Lấy tracked entity
            var tokenEntity = _unitOfWork.ElogbookRepository<ResetSessionToken>().GetById(tokenRecord.ID);

            // Lấy user
            var user = _unitOfWork.ElogbookRepository<User>().GetById(tokenEntity.UserID);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            // Cập nhật password
            user.Password = RuntimeManager.GeneratePasswordHash(request.Password);
            user.UpdatedAt = DateTime.Now;
            _unitOfWork.ElogbookRepository<User>().Update(user);

            // Invalidate token đã dùng
            tokenEntity.Used = true;
            tokenEntity.UpdatedAt = DateTime.Now;
            _unitOfWork.ElogbookRepository<ResetSessionToken>().Update(tokenEntity);

            // Invalidate tất cả OTP cũ của user
            var oldOtps = _unitOfWork.ElogbookRepository<PasswordResetOtp>()
                .GetQuery(x => x.UserID == user.ID && !x.Used && !x.IsDeleted)
                .ToList();
            foreach (var otp in oldOtps)
            {
                otp.Used = true;
                _unitOfWork.ElogbookRepository<PasswordResetOtp>().Update(otp);
            }

            _unitOfWork.Complete();

            // Gửi email xác nhận đổi mật khẩu thành công
            try
            {
                SendPasswordChangedEmail(user.Email, user.FullName);
            }
            catch (Exception ex)
            {
                // Không throw — email xác nhận là optional, không ảnh hưởng luồng chính
                LogCustom.LogMessage(Core.Enums.TrackingLogLevel.ERROR, nameof(PasswordResetService),
                    "Gửi email xác nhận đổi mật khẩu thất bại: " + ex.ToString());
            }
        }

        /// <inheritdoc/>
        public void ResendOtp(ResendOtpRequest request, string ipAddress, string userAgent)
        {
            // Reuse ForgotPassword — tự động invalidate OTP cũ và tạo mới
            ForgotPassword(new ForgotPasswordRequest { Email = request?.Email }, ipAddress, userAgent);
        }

        #region Private helpers

        /// <summary>
        /// Tạo OTP 6 chữ số ngẫu nhiên bảo mật
        /// </summary>
        private static string GenerateOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                // Lấy số nguyên dương, mod 1_000_000 để ra 6 chữ số
                var value = (int)(BitConverter.ToUInt32(bytes, 0) % 1_000_000);
                return value.ToString("D6");
            }
        }

        /// <summary>
        /// Tạo secure token ngẫu nhiên (GUID + random bytes)
        /// </summary>
        private static string GenerateSecureToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
        }

        /// <summary>
        /// Tính SHA-256 hash của chuỗi đầu vào, trả về hex lowercase
        /// </summary>
        private static string ComputeSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gửi email chứa mã OTP
        /// </summary>
        private void SendOtpEmail(string toEmail, string fullName, string otp)
        {
            var config = _emailConfigurationService.GetEmailConfiguration();
            if (config == null || string.IsNullOrWhiteSpace(config.EmailServer))
                throw new Exception("Chưa cấu hình email hệ thống.");

            var subject = "Mã xác nhận đặt lại mật khẩu";
            var body = $@"
<p>Xin chào <strong>{fullName}</strong>,</p>
<p>Mã xác nhận đặt lại mật khẩu của bạn là:</p>
<h2 style='letter-spacing: 8px; color: #2563eb;'>{otp}</h2>
<p>Mã có hiệu lực trong <strong>{OTP_EXPIRES_MINUTES} phút</strong>.</p>
<p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
<br/>
<p><em>Hệ thống eLogbook</em></p>";

            SendSmtp(config, toEmail, fullName, subject, body);
        }

        /// <summary>
        /// Gửi email xác nhận password đã được thay đổi thành công
        /// </summary>
        private void SendPasswordChangedEmail(string toEmail, string fullName)
        {
            var config = _emailConfigurationService.GetEmailConfiguration();
            if (config == null || string.IsNullOrWhiteSpace(config.EmailServer))
                return;

            var subject = "Mật khẩu của bạn đã được thay đổi";
            var body = $@"
<p>Xin chào <strong>{fullName}</strong>,</p>
<p>Mật khẩu tài khoản của bạn đã được đặt lại thành công.</p>
<p>Nếu bạn không thực hiện thao tác này, vui lòng liên hệ quản trị viên ngay lập tức.</p>
<br/>
<p><em>Hệ thống eLogbook</em></p>";

            SendSmtp(config, toEmail, fullName, subject, body);
        }

        /// <summary>
        /// Gửi email qua SMTP
        /// </summary>
        private static void SendSmtp(EmailConfigurationResponseModel config, string toEmail, string toName, string subject, string htmlBody)
        {
            var smtp = new SmtpClient
            {
                Host = config.EmailServer,
                Port = config.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(config.UserName, config.PassWord),
                EnableSsl = true,
                Timeout = 30000,
            };

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(config.EmailAddress);
                mail.To.Add(new MailAddress(toEmail, toName));
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Body = htmlBody;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                smtp.Send(mail);
            }
        }

        #endregion
    }
}
