namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWarnAndSafe")]
    public partial class LctWarnAndSafe : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Nội dung nguy hiểm
        /// </summary>
        [Required]
        public string WarningContent { get; set; }

        /// <summary>
        /// Biện pháp an toàn
        /// </summary>
        [Required]
        public string SafetyMeasureContent { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
