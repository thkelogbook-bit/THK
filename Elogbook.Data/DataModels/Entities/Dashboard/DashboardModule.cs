using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.Dashboard
{
    [Table("DashboardModule")]
    public partial class DashboardModule : EntityBase
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ModuleValue { get; set; }

        public int Order { get; set; }

        public bool IsActive { get; set; }
    }
}
