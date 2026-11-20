using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.SyncAMs;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Đồng bộ dữ liệu AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amMobileSync")]
    public class AMMobileSyncDataController : BaseApiController
    {
        private readonly IAmMobileDataSyncBusiness _amMobileSyncDataBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amMobileSyncDataBusiness"></param>
        public AMMobileSyncDataController(IAmMobileDataSyncBusiness amMobileSyncDataBusiness)
        {
            _amMobileSyncDataBusiness = amMobileSyncDataBusiness;
        }

        /// <summary>
        /// Đồng bộ dữ liệu AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("synAM")]
        [HttpPost]
        public ResponseBaseModel SyncAM([FromBody] MobileAutonomousMaintenanceSyncDataModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _amMobileSyncDataBusiness.SyncAM(requestModel);
        }
    }
}
