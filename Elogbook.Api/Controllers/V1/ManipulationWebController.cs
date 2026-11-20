using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý phiếu thao tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/manipulation")]
    public class ManipulationWebController : BaseApiController
    {
        private readonly IManipulationTicketBusiness _manipulationTicketBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manipulationTicketBusiness"></param>
        /// <param name="commonBusiness"></param>
        public ManipulationWebController(IManipulationTicketBusiness manipulationTicketBusiness,
            ICommonBusiness commonBusiness)
        {
            _manipulationTicketBusiness = manipulationTicketBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Trả về tất cả phiếu thao tác trên Web theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_PTT
        /// <returns></returns>
        [Route("findManipulationWeb")]
        [HttpPost]
        public ResponseBaseModel FindManipulationWeb([FromBody] SearchTicketWebModel requestModel)
        {
            return _manipulationTicketBusiness.FindManipulationWeb(requestModel);
        }

        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteFinalTicket([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_TPTT_ADMIN)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _manipulationTicketBusiness.DeleteFinalTicket(id, CurrentUser.ID);
        }

        /// <summary>
        /// Xác nhận hoàn thành phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("completeManipulation")]
        [HttpPost]
        public ResponseModel<bool> CompleteManipulationWeb([FromBody] CompleteManipulationModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _manipulationTicketBusiness.CompleteManipulationWeb(requestModel);
        }

        /// <summary>
        /// In tài liệu phiếu công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu công tác</param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        public HttpResponseMessage ExportPdfFile([FromUri] int id)
        {
            return _manipulationTicketBusiness.ExportPdfFile(id);
        }

        /// <summary>
        /// Xuất file danh sách Phiếu thao tác
        /// </summary>
        /// <returns></returns>
        [Route("pttExport")]
        [HttpPost]
        public HttpResponseMessage PttExport([FromBody] SearchTicketWebModel requestModel)
        {
            return _manipulationTicketBusiness.PttExport(requestModel);
        }
    }
}