using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.Widget
{
    /// <summary>
    /// Add, edit widget model
    /// </summary>
    public class AddEditWidgetModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int? Id { get; set; }

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
        public bool IsInOverviewDashboard { get; set; } = false;

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
