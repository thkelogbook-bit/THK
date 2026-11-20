using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý danh mục module dashboard
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/dashboardModule")]
    public class DashboardModuleController : BaseApiController
    {
        private readonly IDashboardModuleBusiness _dashboardModuleBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public DashboardModuleController(IDashboardModuleBusiness dashboardModuleBusiness)
        {
            _dashboardModuleBusiness = dashboardModuleBusiness;
        }

        /// <summary>
        /// Lấy danh sách module đang active — dùng cho dropdown chọn module trong form tài liệu
        /// </summary>
        [Route("modules")]
        [HttpGet]
        public ResponseModel<List<DashboardModuleModel>> GetActiveModules()
        {
            return _dashboardModuleBusiness.GetActiveModules();
        }
    }
}
