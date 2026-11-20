using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý phiếu đăng ký công tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/registrationTicket")]
    public class RegistrationTicketController : BaseApiController
    {
        private readonly IRegistrationTicketBusiness _registrationTicketBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="registrationTicketBusiness"></param>
        /// <param name="commonBusiness"></param>
        public RegistrationTicketController(IRegistrationTicketBusiness registrationTicketBusiness,
            ICommonBusiness commonBusiness)
        {
            _registrationTicketBusiness = registrationTicketBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Thêm mới phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> AddRegistrationTicket([FromBody] AddEditRegistrationTicketModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_ĐKCTĐ)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_ĐKCTTCNH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationTicketBusiness.AddRegistrationTicket(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_ĐKCTĐ - EnumUserPermission.VIEW_ĐKCTTCNH
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetRegistrationTickets([FromBody] SearchRegistrationTicketModel requestModel)
        {
            return _registrationTicketBusiness.GetRegistrationTickets(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu đăng ký công tác theo id
        /// </summary>
        /// <param name="id">mã phiếu công tác</param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_ĐKCTĐ - EnumUserPermission.VIEW_ĐKCTTCNH
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<RegistrationTicketModel> GetRegistrationTicketById([FromUri] int id)
        {
            return _registrationTicketBusiness.GetRegistrationTicketById(id);
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu đăng ký công tác (Chờ cấp phiếu công tác/Rút phiếu đăng ký)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSendTicket"></param>
        /// <returns></returns>
        [Route("updateStatus")]
        [HttpPut]
        public ResponseModel<bool> UpdateStatusRegistrationTicket([FromUri] int id, bool isSendTicket)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_ĐKCTĐ)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_ĐKCTTCNH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _registrationTicketBusiness.UpdateStatusRegistrationTicket(id, isSendTicket, CurrentUser.ID);
        }

        /// <summary>
        /// Cập nhật phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateRegistrationTicket([FromBody] AddEditRegistrationTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_ĐKCTĐ)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_ĐKCTTCNH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationTicketBusiness.UpdateRegistrationTicket(requestModel);
        }

        /// <summary>
        /// Xoá phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteRegistrationTicket([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_ĐKCTĐ)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_ĐKCTTCNH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _registrationTicketBusiness.DeleteRegistrationTicket(id, CurrentUser.ID);
        }

        /// <summary>
        /// Tìm kiếm trạng thái phiếu đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_ĐKCTĐ - EnumUserPermission.VIEW_ĐKCTTCNH
        [Route("getStatusHistory/{id}")]
        [HttpGet]
        public ResponseBaseModel GetStatusRegistrationTicketById([FromUri] int id)
        {
            return _registrationTicketBusiness.GetStatusRegistrationTicketById(id);
        }

        /// <summary>
        /// In tài liệu phiếu đăng ký công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu đăng ký công tác</param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_ĐKCTĐ, EnumUserPermission.EDIT_ĐKCTTCNH)]
        public HttpResponseMessage ExportPdfFile([FromUri] int id)
        {
            return _registrationTicketBusiness.ExportPdfFile(id);
        }

        /// <summary>
        /// Tải file mẫu thông tin phiếu đăng ký công tác
        /// </summary>
        /// <returns>File</returns>
        [Route("downloadFileTemplate")]
        [HttpGet]
        public HttpResponseMessage DownloadFileTemplate()
        {
            return _registrationTicketBusiness.DownloadFileTemplate();
        }
    }
}