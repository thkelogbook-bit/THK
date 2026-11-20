namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctTicketStatusHistory")]
    public partial class LctTicketStatusHistory : EntityBase
    {
        /// <summary>
        /// Lệnh đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual LctRegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Lý do huỷ
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string Reason { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

        /// <summary>
        /// Trạng thái lệnh đăng ký
        /// </summary>
        public int StatusValue { get; set; }
        [ForeignKey(nameof(StatusValue))]
        [MapperIgnore]
        public virtual LctTicketStatus TicketStatus { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
