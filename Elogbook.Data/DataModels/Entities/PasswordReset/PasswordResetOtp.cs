namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Lưu OTP dùng để reset password qua email
    /// </summary>
    [Table("PasswordResetOtps")]
    public partial class PasswordResetOtp : EntityBase
    {
        /// <summary>
        /// ID người dùng yêu cầu reset
        /// </summary>
        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        [MapperIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// Hash SHA-256 của OTP (không lưu plaintext)
        /// </summary>
        [Required]
        [StringLength(64)]
        public string OtpHash { get; set; }

        /// <summary>
        /// Thời điểm hết hạn OTP
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Đánh dấu OTP đã được sử dụng
        /// </summary>
        [Required]
        public bool Used { get; set; } = false;

        /// <summary>
        /// Số lần nhập OTP sai
        /// </summary>
        [Required]
        public short Attempts { get; set; } = 0;

        /// <summary>
        /// Địa chỉ IP của request (dùng cho audit)
        /// </summary>
        [StringLength(45)]
        public string IpAddress { get; set; }

        /// <summary>
        /// User-Agent của request (dùng cho audit)
        /// </summary>
        [StringLength(512)]
        public string UserAgent { get; set; }
    }
}
