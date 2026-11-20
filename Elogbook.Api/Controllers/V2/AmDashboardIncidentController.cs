using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Job history
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/dashboardIncident")]
    public class AmDashboardIncidentController : BaseApiController
    {
        private readonly IDashboardIncidentBusiness _dashboardIncidentBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardIncidentBusiness"></param>
        public AmDashboardIncidentController(IDashboardIncidentBusiness dashboardIncidentBusiness)
        {
            _dashboardIncidentBusiness = dashboardIncidentBusiness;
        }

        /// <summary>
        /// Bất thường thiết bị - Tổng hợp
        /// </summary>
        /// EnumUserPermission.VIEW_REPORT_BTTB
        /// <returns></returns>
        [Route("Find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.Find(requestModel);
        }

        /// <summary>
        /// Bất thường thiết bị - Chi tiết
        /// </summary>
        /// EnumUserPermission.VIEW_REPORT_BTTB
        /// <returns></returns>
        [Route("FindDetail")]
        [HttpPost]
        public ResponseBaseModel FindDetail([FromBody] SearchPrtReportModel requestModel)
        {
            return _dashboardIncidentBusiness.FindDetail(requestModel);
        }

        /// <summary>
        /// Get All Activity
        /// </summary>
        /// EnumUserPermission.VIEW_DASHBOARD
        /// <returns></returns>
        [Route("AllActivity")]
        [HttpPost]
        public ResponseBaseModel AllActivity([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.AllActivity(requestModel);
        }

        /// <summary>
        /// Get Abnormal Activity
        /// </summary>
        /// EnumUserPermission.VIEW_DASHBOARD
        /// <returns></returns>
        [Route("AbnormalActivity")]
        [HttpPost]
        public ResponseBaseModel AbnormalActivity([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.AbnormalActivity(requestModel);
        }

        /// <summary>
        /// Get Abnormal Activity
        /// </summary>
        /// EnumUserPermission.VIEW_DASHBOARD
        /// <returns></returns>
        [Route("AbnormalTicketInfo")]
        [HttpPost]
        public ResponseBaseModel AbnormalTicketInfo([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.AbnormalTicketInfo(requestModel);
        }

        /// <summary>
        /// Get Top5User
        /// </summary>
        /// EnumUserPermission.VIEW_DASHBOARD
        /// <returns></returns>
        [Route("Top5User")]
        [HttpPost]
        public ResponseBaseModel Top5User([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.Top5User(requestModel);
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// EnumUserPermission.VIEW_DASHBOARD
        /// <returns></returns>
        [Route("Summary")]
        [HttpPost]
        public ResponseModel<DashboardSummaryGroupModel> Summary([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.Summary(requestModel);
        }

        /// <summary>
        /// Xuất file bất thường thiết bị - tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_REPORT_BTTB
        /// <returns></returns>
        [Route("exportGeneralHandle")]
        [HttpPost]
        public HttpResponseMessage ExportGeneralHandle([FromBody] AbnormalIncidentSearch requestModel)
        {
            return _dashboardIncidentBusiness.ExportGeneralHandle(requestModel);
        }

        /// <summary>
        /// Xuất file bất thường thiết bị - chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_REPORT_BTTB
        /// <returns></returns>
        [Route("exportDetailHandle")]
        [HttpPost]
        public HttpResponseMessage ExportDetailHandle([FromBody] SearchPrtReportModel requestModel)
        {
            return _dashboardIncidentBusiness.ExportDetailHandle(requestModel);
        }
    }
}