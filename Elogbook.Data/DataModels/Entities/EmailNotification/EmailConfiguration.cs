namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmailConfiguration")]
    public partial class EmailConfiguration : EntityBase
    {
        /// <summary>
        /// Thư hệ thống
        /// </summary>
        [Required]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Máy chủ
        /// </summary>
        [Required]
        [StringLength(255)]
        public string EmailServer { get; set; }

        /// <summary>
        /// Cổng
        /// </summary>
        [Required]
        public int Port { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        [Required]
        public int EmailType { get; set; }

        /// <summary>
        /// Tài khoản
        /// </summary>
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PassWord { get; set; }
    }
}
