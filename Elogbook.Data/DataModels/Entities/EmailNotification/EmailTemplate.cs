namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmailTemplate")]
    public partial class EmailTemplate : EntityBase
    {
        /// <summary>
        /// Loại sự kiện gửi email
        /// </summary>
        public int NotificationTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(NotificationTypeID))]
        public virtual SystemData NotificationType { get; set; }

        /// <summary>
        /// Các chức năng có sự kiện gửi email
        /// </summary>
        public int ModuleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ModuleID))]
        public virtual SystemData Module { get; set; }

        /// <summary>
        /// Mã email template
        /// </summary>
        public string EmailCode { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        [Required]
        [StringLength(500)]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Nội dung email
        /// </summary>
        [Required]
        public string EmailContent { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        [Required]
        public int Active { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }
    }
}
