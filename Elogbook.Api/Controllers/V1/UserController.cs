using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.User;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý tài khoản nhân viên
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserBusiness _userBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userBusiness"></param>
        /// <param name="commonBusiness"></param>
        public UserController(IUserBusiness userBusiness,
            ICommonBusiness commonBusiness)
        {
            _userBusiness = userBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm người dùng AD
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_TKNV
        [Route("getADUser")]
        [HttpGet]
        public ADReponse GetADUser([FromUri] string userName)
        {
            ADReponse aDReponse = new ADReponse();

            if (_userBusiness.CheckExistUserByUserName(userName.Trim()))
            {
                aDReponse.ErrorCode = StatusCodeCustom.ADUserNotFound;
                aDReponse.ErrorMessage = CommonConstant.ACCOUNT_EXISTING;
            }
            else
            {
                aDReponse = ADHelper.GetActiveDirectoryUser(userName.Trim());
            }

            return aDReponse;
        }

        /// <summary>
        /// Tìm kiến nhân viên theo điều kiện
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_TKNV)]
        public ResponseBaseModel GetUsers([FromBody] SearchUserModel requestBody)
        {
            return _userBusiness.GetUsers(requestBody);
        }

        /// <summary>
        /// Tìm kiếm một nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<UserModel> GetUserById([FromUri] int id)
        {
            return _userBusiness.GetUserById(id);
        }

        /// <summary>
        /// Tìm kiếm một nhân viên ngoài đơn vị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("external/{id}")]
        [HttpGet]
        public ResponseModel<UserModel> GetExternalUserById([FromUri] int id)
        {
            return _commonBusiness.GetExternalUserById(id);
        }

        /// <summary>
        /// Thêm mới nhân viên
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_TKNV)]
        public ResponseModel<int> AddUser([FromBody] AddEditUserModel model)
        {
            model.CurrentUserID = CurrentUser.ID;
            return _userBusiness.AddUser(model);
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng trên web
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateUserProfileWeb")]
        [HttpPut]
        public ResponseModel<bool> UpdateUserProfileWeb([FromBody] UpdateUserProfileWebModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userBusiness.UpdateUserProfileWeb(requestModel);
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng trên mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateUserProfileMobile")]
        [HttpPut]
        public ResponseModel<bool> UpdateUserProfileMobile([FromBody] UpdateUserProfileMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userBusiness.UpdateUserProfileMobile(requestModel);
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateUser([FromBody] AddEditUserModel model)
        {
            if (!model.IsBypassRight)
            {
                ResponseModel<bool> responseModel = new ResponseModel<bool>();
                if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TKNV)).Data)
                {
                    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return responseModel;
                }
            }

            model.CurrentUserID = CurrentUser.ID;
            model.IsMobileSystem = CurrentUser.IsMobileSystem;
            return _userBusiness.UpdateUser(model);
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_TKNV)]
        public ResponseModel<bool> DeleteUser([FromUri] int id)
        {
            return _userBusiness.DeleteUser(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra User đang được sử
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUseParameterGroup([FromUri] int id)
        {
            return _userBusiness.CheckInUseUser(id);
        }

        /// <summary>
        /// Phương thức migrate password tất cả người dùng từ hệ thống cũ
        /// </summary>
        /// <returns></returns>
        [Route("migrationPasswordOldSystem")]
        [HttpGet]
        public ResponseModel<bool> MigrationPasswordOldSystem()
        {
            return _userBusiness.MigrationPasswordOldSystem();
        }

        /// <summary>
        /// Kiểm tra người dùng có tồn tại địa chỉ email không?
        /// </summary>
        /// <returns></returns>
        [Route("checkExistEmail")]
        [HttpGet]
        public ResponseModel<bool> CheckExistEmail()
        {
            return _userBusiness.CheckExistEmail(CurrentUser.ID);
        }

    }
}
