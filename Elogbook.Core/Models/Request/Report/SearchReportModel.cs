using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Response.Report
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchReportModel : BaseListFilter
    {
        /// <summary>
        /// Find by name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Find by description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Find by module type
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// Find active report
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Find is add dashboard
        /// </summary>
        public bool IsAddDashboard { get; set; }
    }
}
