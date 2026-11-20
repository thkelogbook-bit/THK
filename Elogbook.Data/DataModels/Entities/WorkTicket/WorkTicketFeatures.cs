namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("WorkTicketFeatures")]
    public partial class WorkTicketFeatures : EntityBase
    {
        /// <summary>
        /// Tên chức năng
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FeatureName { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
