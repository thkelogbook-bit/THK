using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý sự kiện PCT-Đ, PCT-TCNH
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/ticketEvent")]
    public class TicketEventController : BaseApiController
    {
        private readonly ITicketEventBusiness _ticketEventBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ticketEventBusiness"></param>
        /// <param name="commonBusiness"></param>
        public TicketEventController(ITicketEventBusiness ticketEventBusiness,
            ICommonBusiness commonBusiness)
        {
            _ticketEventBusiness = ticketEventBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Trả về tất cả sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSK
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetTicketEvents([FromBody] SearchTicketEventModel requestModel)
        {
            return _ticketEventBusiness.GetTicketEvents(requestModel);
        }

        /// <summary>
        /// Tìm kiếm sự kiện theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSK
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<TicketEventModel> GetTicketEventById([FromUri] int id)
        {
            return _ticketEventBusiness.GetTicketEventById(id);
        }

        /// <summary>
        /// Thêm mới sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateTicketEvent([FromBody] AddEditTicketEventModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLSK)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _ticketEventBusiness.CreateTicketEvent(requestModel);
        }

        /// <summary>
        /// Cập nhật sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateTicketEvent([FromBody] AddEditTicketEventModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLSK)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _ticketEventBusiness.UpdateTicketEvent(requestModel);
        }

        /// <summary>
        /// Xoá sự kiện
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteTicketEvent([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLSK)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _ticketEventBusiness.DeleteTicketEvent(id, CurrentUser.ID);
        }
    }
}
