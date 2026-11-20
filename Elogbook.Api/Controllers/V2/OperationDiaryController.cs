using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/operationDiary")]
    public class OperationDiaryController : BaseApiController
    {
        private readonly IOperationDiaryBusiness _operationDiaryBusiness;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="operationDiaryBusiness"></param>
        public OperationDiaryController(IOperationDiaryBusiness operationDiaryBusiness)
        {
            _operationDiaryBusiness = operationDiaryBusiness;
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("operationDiarySyncData")]
        [HttpPost]
        public ResponseBaseModel OperationDiarySyncData([FromBody] OperationDiarySyncDataModel requestModel)
        {
            requestModel.ApiVersion = "V2";
            return _operationDiaryBusiness.OperationDiarySyncData(requestModel);
        }
    }
}
