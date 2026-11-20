using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response.Manipulation;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý phiếu thao tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/RegistrationManipulation")]
    public class RegistrationManipulationController : BaseApiController
    {
        private readonly IRegistrationManipulationBusiness _registrationManipulationBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="registrationManipulationBusiness"></param>
        /// <param name="commonBusiness"></param>
        public RegistrationManipulationController(IRegistrationManipulationBusiness registrationManipulationBusiness,
            ICommonBusiness commonBusiness)
        {
            _registrationManipulationBusiness = registrationManipulationBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public ResponseModel<int> Add([FromBody] AddEditTicketModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_TPTT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationManipulationBusiness.Add(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_TPTT
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] SearchRegistrationManipulationTicketModel requestModel)
        {
            return _registrationManipulationBusiness.Find(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id">mã phiếu công tác</param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_TPTT
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<ManipulationTicketDetailModel> Find([FromUri] int id)
        {
            return _registrationManipulationBusiness.GetManipulationTicketDetailById(id);
        }

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        public ResponseModel<bool> Update([FromBody] AddEditTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TPTT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationManipulationBusiness.Update(requestModel);
        }

        /// <summary>
        /// Cập nhật phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue">1/Chưa gửi duyệt phiếu-2/Chờ duyệt cấp phiếu-3/Đã thu hồi-4/Không được duyệt phiếu-5/Đã duyệt phiếu-6/Chưa thực hiện-7/Chờ xác nhận hoàn thành-8/Đã hoàn thành-9/Bị huỷ</param>
        /// <param name="description"></param>
        /// <param name="attackments"></param>
        /// <returns></returns>
        [Route("updateStatus")]
        [HttpPut]
        public ResponseModel<bool> UpdateStatus([FromUri] int id, int statusValue, string description, string attackments)
        {
            return _registrationManipulationBusiness.UpdateStatus(id, statusValue, description, attackments, CurrentUser.ID);
        }

        /// <summary>
        /// Xoá phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteTicket([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_TPTT)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_TPTT_ADMIN)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _registrationManipulationBusiness.DeleteTicket(id, CurrentUser.ID);
        }

        /// <summary>
        /// Tìm kiếm trạng thái phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getStatusHistory/{id}")]
        [HttpGet]
        public ResponseBaseModel GetStatusById([FromUri] int id)
        {
            return _registrationManipulationBusiness.GetStatusById(id);
        }

        /// <summary>
        /// Duyệt phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("ApprovalTicket")]
        [HttpPost]
        public ResponseModel<int> AddApprovalTicket([FromBody] AddManipulationTicketInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationManipulationBusiness.AddApprovalTicket(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findTemplate")]
        [HttpPost]
        public ResponseBaseModel FindTemplate([FromBody] Core.Models.Request.Manipulation.SearchTemplateTicketModel requestModel)
        {
            return _registrationManipulationBusiness.FindTemplate(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id">mã phiếu công tác</param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_PTTM
        [Route("findTemplate/{id}")]
        [HttpGet]
        public ResponseModel<DetailTemplateTicketModel> FindTemplateId([FromUri] int id)
        {
            return _registrationManipulationBusiness.FindTemplateId(id);
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu đang active
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_PTTM
        [Route("findTemplateActive")]
        [HttpPost]
        public ResponseBaseModel FindTemplateActive()
        {
            return _registrationManipulationBusiness.FindTemplateActive();
        }

        /// <summary>
        /// tạo phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addTemplateTicket")]
        [HttpPost]
        public ResponseModel<int> AddTemplateTicket([FromBody] AddEditTemplateTicketModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_PTTM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationManipulationBusiness.AddTemplateTicket(requestModel);
        }

        /// <summary>
        /// cập nhật phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateTemplateTicket")]
        [HttpPut]
        public ResponseModel<bool> UpdateTemplateTicket([FromBody] AddEditTemplateTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_PTTM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationManipulationBusiness.UpdateTemplateTicket(requestModel);
        }

        /// <summary>
        /// Xoá phiếu thao tác mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteTemplateTicket")]
        [HttpDelete]
        public ResponseModel<bool> DeleteTemplateTicket([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_PTTM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _registrationManipulationBusiness.DeleteTemplateTicket(id, CurrentUser.ID);
        }
    }
}