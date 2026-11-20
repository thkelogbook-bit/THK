using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.Report
{
    [Table("Report")]
    public partial class Report : EntityBase
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ModuleType { get; set; }

        public bool IsAddToDashboard { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

    }
}
