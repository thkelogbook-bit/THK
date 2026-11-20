using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Identity;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.User;
using Microsoft.Web.Http;
using System;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/auth")]
    public class AuthenticationController : BaseApiController
    {
        /// <summary>
        /// User business
        /// </summary>
        private readonly IUserBusiness _userBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AuthenticationController(IUserBusiness userBusiness, ICommonBusiness commonBusiness)
        {
            _userBusiness = userBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public ResponseModel<CurrentUserModel> Login([FromBody] LoginRequestModel requestModel)
        {
            var responseData = new ResponseModel<CurrentUserModel>();
            try
            {
                var invalid = false;

                if (string.IsNullOrWhiteSpace(requestModel.Username))
                {
                    responseData.ErrorMessages.Add("Tên đăng nhập là bắt buộc!");
                    responseData.Data = null;
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                    return responseData;
                }

                if (string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    responseData.ErrorMessages.Add("Mật khẩu là bắt buộc!");
                    responseData.Data = null;
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                    return responseData;
                }

                var userInfo = _userBusiness.GetCurrentUser(requestModel.Username);
                if (userInfo != null && userInfo.ID > 0)
                {
                    if (userInfo.NumberLoginFailed >= 5)
                    {
                        responseData.Data = null;
                        responseData.ErrorMessages.Add(CommonConstant.LOCK_ACCOUNT);
                        responseData.StatusCode = StatusCodeCustom.InvalidAccount;

                        return responseData;
                    }

                    responseData.Data = userInfo;
                    // Nếu người dùng AD
                    if (userInfo.SourceType.Value.GetValueOrDefault(0) == (int)UserSource.ADUser)
                    {
                        // Xác thực người dùng trên AD server
                        var aDUserInfo = ADHelper.ValidateADUser(requestModel.Username, requestModel.Password);
                        if (aDUserInfo != null)
                        {
                            if (userInfo.IsExistActiveRole && userInfo.Status == (int)UserStatus.Active)
                            {
                                switch (userInfo.AccountType.Value.GetValueOrDefault(0))
                                {
                                    case (int)UserType.TwoSystem:
                                        invalid = false;
                                        break;
                                    case (int)UserType.AdminPortal:
                                        invalid = requestModel.IsMobile;
                                        break;
                                    case (int)UserType.MobileApp:
                                        invalid = !requestModel.IsMobile;
                                        break;
                                }
                            }
                            else
                            {
                                invalid = true;
                            }

                            if (!invalid)
                            {
                                responseData.Data.FullName = aDUserInfo.FullName;
                                // Cập nhật thông tin người dùng vào database
                                var model = new AddEditUserModel
                                {
                                    ID = userInfo.ID,
                                    UserName = aDUserInfo.UserName,
                                    FullName = aDUserInfo.FullName,
                                    Password = requestModel.Password,
                                    IsLoginAD = true,
                                };
                                _userBusiness.UpdateUser(model);
                            }
                            else
                            {
                                responseData.Data = null;
                                responseData.ErrorMessages.Add(requestModel.IsMobile ? CommonConstant.NOT_HAS_PERMISSION_MOBILE : CommonConstant.NOT_HAS_PERMISSION);
                                responseData.StatusCode = StatusCodeCustom.NoPermission;
                                return responseData;
                            }
                        }
                        else
                        {
                            // Tài khoản hoặc mật khẩu chưa chính xác
                            invalid = true;
                            // Nếu đăng nhập sai mật khẩu thì tăng số lần đăng nhập lỗi lên 1
                            // Nếu sai 5 lần thì báo log tài khoản
                            _userBusiness.HandleLoginFailed(userInfo.ID, true);
                        }
                    }
                    // Nếu người dùng Admin Portal
                    else
                    {
                        if (RuntimeManager.ValidatePassword(requestModel.Password, userInfo.Password))
                        {
                            if (userInfo.IsExistActiveRole && userInfo.Status == (int)UserStatus.Active)
                            {
                                switch (userInfo.AccountType.Value.GetValueOrDefault(0))
                                {
                                    case (int)UserType.TwoSystem:
                                        invalid = false;
                                        break;
                                    case (int)UserType.AdminPortal:
                                        invalid = requestModel.IsMobile;
                                        break;
                                    case (int)UserType.MobileApp:
                                        invalid = !requestModel.IsMobile;
                                        break;
                                }
                            }
                            else
                            {
                                invalid = true;
                            }

                            if (invalid)
                            {
                                responseData.Data = null;
                                responseData.ErrorMessages.Add(requestModel.IsMobile ? CommonConstant.NOT_HAS_PERMISSION_MOBILE : CommonConstant.NOT_HAS_PERMISSION);
                                responseData.StatusCode = StatusCodeCustom.NoPermission;
                                return responseData;
                            }
                        }
                        else
                        {
                            // Tài khoản hoặc mật khẩu chưa chính xác
                            invalid = true;
                            // Nếu đăng nhập sai mật khẩu thì tăng số lần đăng nhập lỗi lên 1
                            // Nếu sai 5 lần thì báo log tài khoản
                            _userBusiness.HandleLoginFailed(userInfo.ID, true);
                        }
                    }
                }
                else
                {
                    // Tài khoản hoặc mật khẩu chưa chính xác
                    invalid = true;
                }

                if (invalid)
                {
                    responseData.Data = null;
                    responseData.ErrorMessages.Add(CommonConstant.INVALID_ACCOUNT);
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                }
                else
                {
                    // Nếu đăng nhập thành công thì reset số lần đăng nhập lỗi về 0
                    _userBusiness.HandleLoginFailed(userInfo.ID, false);
                    var customIdentity = new CustomIdentity
                    {
                        ID = responseData.Data.ID,
                        Username = responseData.Data.UserName,
                        FullName = responseData.Data.FullName,
                        CompanyID = responseData.Data.Company.Value.GetValueOrDefault(0),
                        ProductionPlantID = responseData.Data.ProductionPlant.Value.GetValueOrDefault(0),
                        IsMobileSystem = requestModel.IsMobile
                    };

                    // Tạo token cho người dùng
                    var tokenInfo = JwtManager.GenerateToken(customIdentity);
                    responseData.Data.AccessToken = tokenInfo.AccessToken;
                    responseData.Data.TokenExpireTime = tokenInfo.TokenExpireTime;
                    var userRolesInfo = _commonBusiness.GetUserRight(responseData.Data.ID);
                    if (userRolesInfo != null)
                    {
                        responseData.Data.UsersRight = userRolesInfo.UsersRight;
                        responseData.Data.IsLimited = userRolesInfo.IsLimited;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, ex.ToString());
            }

            return responseData;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [Route("logOut")]
        [HttpPut]
        public ResponseModel<bool> LogOut()
        {
            // Lấy JWT token từ Authorization header
            var authHeader = Request.Headers.Authorization;
            var tokenExpired = string.Empty;
            if (authHeader != null && authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                tokenExpired = authHeader.Parameter;
            }

            return _userBusiness.LogOut(CurrentUser.ID, CurrentUser.Username, tokenExpired);
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("changePassword")]
        [HttpPut]
        public ResponseModel<bool> ChangePassword([FromBody] ChangePasswordModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            requestModel.UserName = CurrentUser.Username;
            return _userBusiness.ChangePassword(requestModel);
        }

        /// <summary>
        /// Lấy token người dùng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getToken")]
        [HttpPost]
        [AllowAnonymous]
        public ResponseModel<UserTokenModel> GetToken([FromBody] LoginRequestModel requestModel)
        {
            var responseData = new ResponseModel<UserTokenModel>();
            try
            {
                var invalid = false;
                if (string.IsNullOrWhiteSpace(requestModel.Username))
                {
                    responseData.ErrorMessages.Add("Tên đăng nhập là bắt buộc!");
                    responseData.Data = null;
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                    return responseData;
                }

                if (string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    responseData.ErrorMessages.Add("Mật khẩu là bắt buộc!");
                    responseData.Data = null;
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                    return responseData;
                }

                var userInfo = _userBusiness.GetCurrentUser(requestModel.Username);
                if (userInfo != null && userInfo.ID > 0)
                {
                    if (userInfo.NumberLoginFailed >= 5)
                    {
                        responseData.Data = null;
                        responseData.ErrorMessages.Add(CommonConstant.LOCK_ACCOUNT);
                        responseData.StatusCode = StatusCodeCustom.InvalidAccount;

                        return responseData;
                    }

                    // Nếu người dùng AD
                    if (userInfo.SourceType.Value.GetValueOrDefault(0) == (int)UserSource.ADUser)
                    {
                        // Xác thực người dùng trên AD server
                        var aDUserInfo = ADHelper.ValidateADUser(requestModel.Username, requestModel.Password);
                        if (aDUserInfo != null)
                        {
                            if (userInfo.IsExistActiveRole && userInfo.Status == (int)UserStatus.Active)
                            {
                                switch (userInfo.AccountType.Value.GetValueOrDefault(0))
                                {
                                    case (int)UserType.TwoSystem:
                                        invalid = false;
                                        break;
                                    case (int)UserType.AdminPortal:
                                        invalid = requestModel.IsMobile;
                                        break;
                                    case (int)UserType.MobileApp:
                                        invalid = !requestModel.IsMobile;
                                        break;
                                }
                            }
                            else
                            {
                                invalid = true;
                            }

                            if (!invalid)
                            {
                                responseData.Data.FullName = aDUserInfo.FullName;
                                // Cập nhật thông tin người dùng vào database
                                var model = new AddEditUserModel
                                {
                                    ID = userInfo.ID,
                                    UserName = aDUserInfo.UserName,
                                    FullName = aDUserInfo.FullName,
                                    Password = requestModel.Password,
                                    IsLoginAD = true,
                                };
                                _userBusiness.UpdateUser(model);
                            }
                            else
                            {
                                responseData.Data = null;
                                responseData.ErrorMessages.Add(requestModel.IsMobile ? CommonConstant.NOT_HAS_PERMISSION_MOBILE : CommonConstant.NOT_HAS_PERMISSION);
                                responseData.StatusCode = StatusCodeCustom.NoPermission;
                                return responseData;
                            }
                        }
                        else
                        {
                            // Tài khoản hoặc mật khẩu chưa chính xác
                            invalid = true;
                            // Nếu đăng nhập sai mật khẩu thì tăng số lần đăng nhập lỗi lên 1
                            // Nếu sai 5 lần thì báo log tài khoản
                            _userBusiness.HandleLoginFailed(userInfo.ID, true);
                        }
                    }
                    // Nếu người dùng Admin Portal
                    else
                    {
                        if (RuntimeManager.ValidatePassword(requestModel.Password, userInfo.Password))
                        {
                            if (userInfo.IsExistActiveRole && userInfo.Status == (int)UserStatus.Active)
                            {
                                switch (userInfo.AccountType.Value.GetValueOrDefault(0))
                                {
                                    case (int)UserType.TwoSystem:
                                        invalid = false;
                                        break;
                                    case (int)UserType.AdminPortal:
                                        invalid = requestModel.IsMobile;
                                        break;
                                    case (int)UserType.MobileApp:
                                        invalid = !requestModel.IsMobile;
                                        break;
                                }
                            }
                            else
                            {
                                invalid = true;
                            }

                            if (invalid)
                            {
                                responseData.Data = null;
                                responseData.ErrorMessages.Add(requestModel.IsMobile ? CommonConstant.NOT_HAS_PERMISSION_MOBILE : CommonConstant.NOT_HAS_PERMISSION);
                                responseData.StatusCode = StatusCodeCustom.NoPermission;
                                return responseData;
                            }
                        }
                        else
                        {
                            // Tài khoản hoặc mật khẩu chưa chính xác
                            invalid = true;
                            // Nếu đăng nhập sai mật khẩu thì tăng số lần đăng nhập lỗi lên 1
                            // Nếu sai 5 lần thì báo log tài khoản
                            _userBusiness.HandleLoginFailed(userInfo.ID, true);
                        }
                    }
                }
                else
                {
                    // Tài khoản hoặc mật khẩu chưa chính xác
                    invalid = true;
                }

                if (invalid)
                {
                    responseData.Data = null;
                    responseData.ErrorMessages.Add(CommonConstant.INVALID_ACCOUNT);
                    responseData.StatusCode = StatusCodeCustom.InvalidAccount;
                }
                else
                {
                    // Nếu đăng nhập thành công thì reset số lần đăng nhập lỗi về 0
                    _userBusiness.HandleLoginFailed(userInfo.ID, false);
                    var customIdentity = new CustomIdentity
                    {
                        ID = userInfo.ID,
                        Username = userInfo.UserName,
                        FullName = userInfo.FullName,
                        CompanyID = userInfo.Company.Value.GetValueOrDefault(0),
                        ProductionPlantID = userInfo.ProductionPlant.Value.GetValueOrDefault(0),
                        IsMobileSystem = requestModel.IsMobile
                    };

                    // Tạo token cho người dùng
                    var tokenInfo = JwtManager.GenerateToken(customIdentity);
                    responseData.Data = new UserTokenModel
                    {
                        ID = userInfo.ID,
                        UserName = userInfo.UserName,
                        FullName = userInfo.FullName,
                        AccessToken = tokenInfo.AccessToken,
                        TokenExpireTime = tokenInfo.TokenExpireTime
                    };
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, ex.ToString());
            }

            return responseData;
        }

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <returns></returns>
        [Route("getUserInfo")]
        [HttpGet]
        public ResponseModel<CurrentUserModel> GetUserInfo()
        {
            var responseData = new ResponseModel<CurrentUserModel>();
            try
            {
                var userInfo = _userBusiness.GetCurrentUser(CurrentUser.Username);
                if (userInfo != null && userInfo.ID > 0)
                {

                    responseData.Data = userInfo;
                    var userRolesInfo = _commonBusiness.GetUserRight(responseData.Data.ID);
                    if (userRolesInfo != null)
                    {
                        responseData.Data.UsersRight = userRolesInfo.UsersRight;
                        responseData.Data.IsLimited = userRolesInfo.IsLimited;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, ex.ToString());
            }

            return responseData;
        }
    }
}
