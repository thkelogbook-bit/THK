using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Microsoft.Web.Http;
using System.Web.Http;
using Hangfire;
using Elogbook.Core.Models.Response.Dashboard;
using System.Collections.Generic;
using Elogbook.Core.Models.Request;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Dashboard
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/dashboards")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardBusiness _dashboardBusiness;


        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="dashboardBusiness"></param>
        public DashboardController(IDashboardBusiness dashboardBusiness)
        {
            _dashboardBusiness = dashboardBusiness;
        }

        /// <summary>
        /// Lấy thông tin dashboard
        /// </summary>
        /// <returns></returns>
        [Route("active")]
        [HttpGet]
        public ResponseModel<List<ListDashboardModuleView>> GetDashboardsActive()
        {
            return _dashboardBusiness.GetDashboards(CurrentUser.ID, false);
        }

        /// <summary>
        /// Lấy thông tin dashboard
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public ResponseModel<List<ListDashboardModuleView>> GetDashboards()
        {
            return _dashboardBusiness.GetDashboards(CurrentUser.ID, true);
        }

        /// <summary>
        /// Lấy thông tin dashboard
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        [Route("info")]
        [HttpGet]
        public ResponseModel<DashboardViewModel> GetInfo([FromParameter("reportId")] int reportId)
        {
            return _dashboardBusiness.GetInfo(reportId);
        }

        /// <summary>
        /// Chỉnh sữa dashboard
        /// </summary>
        /// <param name="dashboardModel"></param>
        /// <returns></returns>
        [Route]
        [HttpPut]
        public ResponseModel<ListDashboardModuleView> Update(EditDashboardModel dashboardModel)
        {
            dashboardModel.CurrentUserID = CurrentUser.ID;
            return _dashboardBusiness.Update(dashboardModel);
        }
    }
}