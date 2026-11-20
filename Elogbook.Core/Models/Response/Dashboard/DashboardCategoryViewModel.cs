
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.Dashboard
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardCategoryViewModel
    {

        /// <summary>
        /// 
        /// </summary>
        public string Category;

        /// <summary>
        /// 
        /// </summary>
        public double Total;

        /// <summary>
        /// 
        /// </summary>
        public List<DashboardInfoViewModel> DataCount = new List<DashboardInfoViewModel>();
    }
}
