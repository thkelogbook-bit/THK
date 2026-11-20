namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Lưu token tạm sau khi verify OTP thành công, dùng để đặt password mới
    /// </summary>
    [Table("ResetSessionTokens")]
    public partial class ResetSessionToken : EntityBase
    {
        /// <summary>
        /// ID người dùng
        /// </summary>
        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        [MapperIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// Hash SHA-256 của reset session token (không lưu plaintext)
        /// </summary>
        [Required]
        [StringLength(64)]
        public string TokenHash { get; set; }

        /// <summary>
        /// Thời điểm hết hạn token (5 phút)
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Đánh dấu token đã được sử dụng
        /// </summary>
        [Required]
        public bool Used { get; set; } = false;
    }
}
