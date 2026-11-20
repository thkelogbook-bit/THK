using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.Dashboard;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<ListDashboardModuleView> GetDashboards(int userId, bool includeInactive = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        DashboardViewModel GetInfo(int reportId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboardModel"></param>
        /// <returns></returns>
        ListDashboardModuleView Update(EditDashboardModel dashboardModel);

    }
}
