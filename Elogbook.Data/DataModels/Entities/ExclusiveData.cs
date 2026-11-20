namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("ExclusiveData")]
    public partial class ExclusiveData : EntityBase
    {
        /// <summary>
        /// Giá trị lock
        /// </summary>
        public int TicketNumber { get; set; }

        /// <summary>
        /// Loại phiếu: PTCNH: Thủy cơ nhiệt hóa, PCTD: Công tác điện, PTT: Phiếu thao tác
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
