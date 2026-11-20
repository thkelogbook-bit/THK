using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.InspectionReport;
using Elogbook.Core.Models.Response.InspectionReport;
using Microsoft.Web.Http;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý biên bản khảo sát hiện trường
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/inspectionReport")]
    public class InspectionReportController : BaseApiController
    {
        private readonly IInspectionReportBusiness _inspectionReportBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inspectionReportBusiness"></param>
        public InspectionReportController(IInspectionReportBusiness inspectionReportBusiness)
        {
            _inspectionReportBusiness = inspectionReportBusiness;
        }

        /// <summary>
        /// Tìm kiếm những BBKSHT theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetAll([FromBody] SearchInspectionReportModel requestModel)
        {
            return _inspectionReportBusiness.GetAll(requestModel);
        }

        /// <summary>
        /// Tạo mới BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<InspectionReportDetailsModel> Add([FromBody] AddEditInspectionReporModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _inspectionReportBusiness.Add(requestModel);
        }

        /// <summary>
        /// Chỉnh sửa BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<InspectionReportDetailsModel> Update([FromBody] AddEditInspectionReporModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _inspectionReportBusiness.Update(requestModel);
        }

        /// <summary>
        /// Thông tin chi tiết BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<InspectionReportDetailsModel> Details([FromUri] int id)
        {
            return _inspectionReportBusiness.Details(id);
        }

        /// <summary>
        /// Xóa BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromUri] int id)
        {
            return _inspectionReportBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// Ký xác nhận
        /// </summary>
        /// <param name="signgingModel"></param>
        /// <returns></returns>
        [Route("signing")]
        [AllowAnonymous]
        [HttpPost]
        public ResponseModel<UserSigningModel> Signging([FromBody] SigngingModel signgingModel)
        {
            return _inspectionReportBusiness.Signging(signgingModel);
        }

        /// <summary>
        /// In BBKSHT ra file PDF
        /// </summary>
        /// <param name="id">ID BBKSHT</param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        public HttpResponseMessage ExportPdfFile([FromUri] int id)
        {
            return _inspectionReportBusiness.ExportPdfFile(id);
        }
    }
}