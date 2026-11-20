using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.Dashboard;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDashboardBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ResponseModel<List<ListDashboardModuleView>> GetDashboards(int userId, bool includeInactive = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        ResponseModel<DashboardViewModel>  GetInfo(int reportId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboardModel"></param>
        /// <returns></returns>
        ResponseModel<ListDashboardModuleView> Update(EditDashboardModel dashboardModel);

    }
}
