
namespace Elogbook.Core.Models.Response.Widget
{
    /// <summary>
    /// Widget view model
    /// </summary>
    public class WidgetViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WidgetType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChartType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReportId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInOverviewDashboard { get; set; } = false;

    }
}
