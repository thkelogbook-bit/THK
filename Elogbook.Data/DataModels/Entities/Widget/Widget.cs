using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.Widget
{
    [Table("Widget")]
    public partial class Widget : EntityBase
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(150)]
        public string WidgetType { get; set; }

        [StringLength(150)]
        public string ChartType { get; set; }

        [StringLength(50)]
        public string ModuleType { get; set; }

        public int ReportId { get; set; }

        public int Order { get; set; }

        public bool IsInOverviewDashboard { get; set; } = false;

    }
}
