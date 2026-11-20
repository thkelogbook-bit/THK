namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkTicketFeatureLink")]
    public partial class LctWorkTicketFeatureLink : EntityBase
    {
        /// <summary>
        /// ID chức năng
        /// </summary>
        public int FeatureID { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FeatureID))]
        public virtual LctWorkTicketFeatures WorkTicketFeatures { get; set; }
    }
}
