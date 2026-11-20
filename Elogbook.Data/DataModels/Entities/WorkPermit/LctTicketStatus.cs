namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctTicketStatus")]
    public partial class LctTicketStatus : EntityBase
    {
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        [Required]
        [StringLength(255)]
        public string StatusName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        [StringLength(50)]
        public string TicketType { get; set; }


        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
