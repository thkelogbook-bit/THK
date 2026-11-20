using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Xuất dữ liệu file excel
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/export")]
    public class ExcelExportController : BaseApiController
    {
        private readonly IExcelExportBusiness _excelExportBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="excelExportBusiness"></param>
        public ExcelExportController(IExcelExportBusiness excelExportBusiness)
        {
            _excelExportBusiness = excelExportBusiness;
        }

        /// <summary>
        /// Xuất dữ liệu file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("exportExcel")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage ExportFile([FromBody] ExportExcelRequestModel requestModel)
        {
            return _excelExportBusiness.ExportExcelFile(requestModel);
        }
    }
}
