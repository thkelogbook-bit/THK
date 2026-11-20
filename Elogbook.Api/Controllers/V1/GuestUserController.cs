using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý người ngoài hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/guestUser")]
    public class GuestUserController : BaseApiController
    {
        private readonly IGuestUserBusiness _guestUserBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="guestUserBusiness"></param>
        /// <param name="commonBusiness"></param>
        public GuestUserController(IGuestUserBusiness guestUserBusiness,
            ICommonBusiness commonBusiness)
        {
            _guestUserBusiness = guestUserBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Danh sách người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_NDNHT
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetGuestUsers([FromBody] SearchGuestUserModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Content = null;
                return responseModel;
            }

            return _guestUserBusiness.GetGuestUsers(requestModel);
        }

        /// <summary>
        /// Chi tiết người ngoài hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_NDNHT
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<GuestUserModel> GetGuestUserById([FromUri] int id)
        {
            return _guestUserBusiness.GetGuestUserById(id);
        }

        /// <summary>
        /// Thêm mới người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<GuestUserModel> AddGuestUser([FromBody] AddEditGuestUserModel requestModel)
        {
            ResponseModel<GuestUserModel> responseModel = new ResponseModel<GuestUserModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_NDNHT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Data = null;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _guestUserBusiness.AddGuestUser(requestModel);
        }

        /// <summary>
        /// Cập nhập người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateGuestUser([FromBody] AddEditGuestUserModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_NDNHT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Data = false;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _guestUserBusiness.UpdateGuestUser(requestModel);
        }

        /// <summary>
        /// Xóa người ngoài hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteGuestUser([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_NDNHT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _guestUserBusiness.DeleteGuestUser(id, CurrentUser.ID);
        }

        /// <summary>
        /// Người ngoài hệ thống đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUseGuestUser([FromUri] int id)
        {
            return _guestUserBusiness.CheckInUseGuestUser(id);
        }
    }
}