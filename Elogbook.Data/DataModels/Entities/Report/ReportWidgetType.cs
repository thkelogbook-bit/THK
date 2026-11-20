using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.Report
{
    [Table("ReportWidgetType")]
    public class ReportWidgetType : EntityBase
    {
        public long ReportId { get; set; }

        [Required]
        [StringLength(150)]
        public string WidgetType { get; set; }

        [Required]
        [StringLength(150)]
        public string ChartType { get; set; }
    }
}
