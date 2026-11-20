using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.Dashboard
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title;

        /// <summary>
        /// 
        /// </summary>
        public List<DashboardCategoryViewModel> Data = new List<DashboardCategoryViewModel>();
    }
}
