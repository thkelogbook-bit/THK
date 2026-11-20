namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("WorkTicketFeatureLink")]
    public partial class WorkTicketFeatureLink : EntityBase
    {
        /// <summary>
        /// ID chức năng
        /// </summary>
        public int FeatureID { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FeatureID))]
        public virtual WorkTicketFeatures WorkTicketFeatures { get; set; }

    }
}
