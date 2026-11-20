using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.User;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Elogbook.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserService _userService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UserBusiness(IUserService userService)
        {
            _userService = userService;
        }

        public ResponseBaseModel GetUsers(SearchUserModel requestModel)
        {
            var responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _userService.GetUsers(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<UserModel> GetUserById(int id)
        {
            var responseModel = new ResponseModel<UserModel>();

            try
            {
                responseModel.Data = _userService.GetUserById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public CurrentUserModel GetCurrentUser(string username)
        {
            CurrentUserModel currentUserModel = null;

            try
            {
                currentUserModel = _userService.GetCurrentUser(username);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"username: {username} " + Environment.NewLine + ex.ToString());
            }

            return currentUserModel;
        }

        public ResponseModel<int> AddUser(AddEditUserModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel, true))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    var user = GetCurrentUser(requestModel.UserName);
                    if (user == null)
                    {
                        responseModel.Data = _userService.AddUser(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_USER);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateUserProfileWeb(UpdateUserProfileWebModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidDataWeb(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _userService.UpdateUserProfileWeb(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateUserProfileMobile(UpdateUserProfileMobileModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidDataMobile(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _userService.UpdateUserProfileMobile(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateUser(AddEditUserModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel, false))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _userService.UpdateUser(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteUser(int id, int userID)
        {
            var responseModel = new ResponseModel<bool>();
            // Đối với tài khoản đã thực hiện thay đổi dữ liệu của hệ thống thì không thể xóa
            // (ghi nhật ký vận hành, ghi thông số vận hành, ...) hiển thị thông báo "Tài khoản này không thể xóa"
            try
            {
                if (id == userID)
                {
                    responseModel.Data = false;
                    responseModel.StatusCode = -1;
                    responseModel.ErrorMessages.Add("Không thể xóa người dùng đang đăng nhập.");
                    return responseModel;
                }

                //if (!_userService.CheckInUseUser(id))
                //{
                responseModel.Data = _userService.DeleteUser(id, userID);
                if (!responseModel.Data)
                {
                    responseModel.StatusCode = -1;
                }
                //}
                //else
                //{
                //    responseModel.Data = false;
                //    responseModel.StatusCode = -1;
                //    responseModel.ErrorMessages.Add(CommonConstant.MSG_CANNOT_DELETE_USER);
                //}
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public bool CheckExistUserByUserName(string userName)
        {
            try
            {
                return _userService.CheckExistUserByUserName(userName);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"userName: {userName} " + Environment.NewLine + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Người dùng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditUserModel requestModel, bool isAddMode)
        {
            // Họ và tên
            if (string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                _errorMessages.Add(CommonConstant.FULLNAME_REQUIRED);
            }

            // Tên đăng nhập
            if (string.IsNullOrWhiteSpace(requestModel.UserName))
            {
                _errorMessages.Add(CommonConstant.USERNAME_REQUIRED);
            }

            // Mật khẩu bắt buộc khi người dùng được thêm mới không nằm trong AD
            if (string.IsNullOrWhiteSpace(requestModel.Password) && requestModel.SourceType == (int)UserSource.AdminPortal && requestModel.ID < 1)
            {
                _errorMessages.Add(CommonConstant.PASSWORD_REQUIRED);
            }

            // Kiểm tra độ mạnh của mật khẩu
            if (!string.IsNullOrWhiteSpace(requestModel.Password))
            {
                IsStrongPassword(requestModel.Password);
            }

            // Vai trò
            if (string.IsNullOrWhiteSpace(requestModel.UserRoleIDs))
            {
                _errorMessages.Add(CommonConstant.USER_ROLE_REQUIRED);
            }

            // Công ty
            if (requestModel.CompanyID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_COMPANY_REQUIRED);
            }

            // Chức vụ
            if (string.IsNullOrWhiteSpace(requestModel.PositionName))
            {
                _errorMessages.Add(CommonConstant.POSITIONNAME_REQUIRED);
            }

            // Số CMND/CCCD
            if (string.IsNullOrWhiteSpace(requestModel.NationalID))
            {
                _errorMessages.Add(CommonConstant.NATIONALID_REQUIRED);
            }

            // Hình ảnh số CMND/CCCD
            if (requestModel.NationalAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.NATIONALATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRankCard))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKCARD_REQUIRED);
            }

            // Bậc an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn điện
            if (requestModel.ElectricRankAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn lao động
            if (string.IsNullOrWhiteSpace(requestModel.LaborCard))
            {
                _errorMessages.Add(CommonConstant.LABORCARD_REQUIRED);
            }

            // Bậc thợ
            if (string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                _errorMessages.Add(CommonConstant.LABORRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn lao động
            if (requestModel.LaborCardAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.LABORCARDATTACHMENTID_REQUIRED);
            }

            // Chữ ký
            if (string.IsNullOrWhiteSpace(requestModel.Signature))
            {
                _errorMessages.Add(CommonConstant.SIGNATURE_REQUIRED);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.NationalID) && _userService.CheckDuplicateData(requestModel.ID, requestModel.NationalID, isAddMode, fieldType: 1))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_NATIONALID);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.ElectricRankCard, isAddMode, fieldType: 2))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_ELECTRICRANKCARD);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.LaborCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.LaborCard, isAddMode, fieldType: 3))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_LABORCARD);
            }

            if (requestModel.SigningMethodID < 1)
            {
                _errorMessages.Add(CommonConstant.SIGNINGMETHODID_REQUIRED);
            }
            else
            {
                if (!_userService.CheckValidSigningMethod(requestModel.SigningMethodID))
                {
                    _errorMessages.Add(CommonConstant.SIGNINGMETHOD_NOT_VALID);
                }
            }

            // Bậc an toàn điện (x/y: x <= y) ví dụ: 1/5 
            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                string[] arrValue = requestModel.ElectricRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.ELECTRICRANK_NOT_VALID);
                }

            }
            // Bậc thợ (x/y: x <= y) ví dụ: 1/7 
            if (!string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                string[] arrValue = requestModel.LaborRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.LABORRANK_NOT_VALID);
                }
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidDataWeb(UpdateUserProfileWebModel requestModel)
        {
            // Họ và tên
            if (string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                _errorMessages.Add(CommonConstant.FULLNAME_REQUIRED);
            }

            // Số CMND/CCCD
            if (string.IsNullOrWhiteSpace(requestModel.NationalID))
            {
                _errorMessages.Add(CommonConstant.NATIONALID_REQUIRED);
            }

            // Hình ảnh số CMND/CCCD
            if (requestModel.NationalAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.NATIONALATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRankCard))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKCARD_REQUIRED);
            }

            // Bậc an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn điện
            if (requestModel.ElectricRankAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn lao động
            if (string.IsNullOrWhiteSpace(requestModel.LaborCard))
            {
                _errorMessages.Add(CommonConstant.LABORCARD_REQUIRED);
            }

            // Bậc thợ
            if (string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                _errorMessages.Add(CommonConstant.LABORRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn lao động
            if (requestModel.LaborCardAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.LABORCARDATTACHMENTID_REQUIRED);
            }

            // Chữ ký
            if (string.IsNullOrWhiteSpace(requestModel.Signature))
            {
                _errorMessages.Add(CommonConstant.SIGNATURE_REQUIRED);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.NationalID) && _userService.CheckDuplicateData(requestModel.ID, requestModel.NationalID, false, fieldType: 1))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_NATIONALID);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.ElectricRankCard, false, fieldType: 2))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_ELECTRICRANKCARD);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.LaborCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.LaborCard, false, fieldType: 3))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_LABORCARD);
            }

            if (requestModel.SigningMethodID < 1)
            {
                _errorMessages.Add(CommonConstant.SIGNINGMETHODID_REQUIRED);
            }
            else
            {
                if (!_userService.CheckValidSigningMethod(requestModel.SigningMethodID))
                {
                    _errorMessages.Add(CommonConstant.SIGNINGMETHOD_NOT_VALID);
                }
            }

            // Bậc an toàn điện (x/y: x <= y) ví dụ: 1/5 
            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                string[] arrValue = requestModel.ElectricRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.ELECTRICRANK_NOT_VALID);
                }

            }
            // Bậc thợ (x/y: x <= y) ví dụ: 1/7 
            if (!string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                string[] arrValue = requestModel.LaborRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.LABORRANK_NOT_VALID);
                }
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidDataMobile(UpdateUserProfileMobileModel requestModel)
        {
            // Họ và tên
            if (string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                _errorMessages.Add(CommonConstant.FULLNAME_REQUIRED);
            }

            // Số CMND/CCCD
            if (string.IsNullOrWhiteSpace(requestModel.NationalID))
            {
                _errorMessages.Add(CommonConstant.NATIONALID_REQUIRED);
            }

            // Hình ảnh số CMND/CCCD
            if (requestModel.NationalAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.NATIONALATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRankCard))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKCARD_REQUIRED);
            }

            // Bậc an toàn điện
            if (string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn điện
            if (requestModel.ElectricRankAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.ELECTRICRANKATTACHMENTID_REQUIRED);
            }

            // Thẻ an toàn lao động
            if (string.IsNullOrWhiteSpace(requestModel.LaborCard))
            {
                _errorMessages.Add(CommonConstant.LABORCARD_REQUIRED);
            }

            // Bậc thợ
            if (string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                _errorMessages.Add(CommonConstant.LABORRANK_REQUIRED);
            }

            // Hình ảnh thẻ an toàn lao động
            if (requestModel.LaborCardAttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.LABORCARDATTACHMENTID_REQUIRED);
            }

            // Chữ ký
            if (string.IsNullOrWhiteSpace(requestModel.Signature))
            {
                _errorMessages.Add(CommonConstant.SIGNATURE_REQUIRED);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.NationalID) && _userService.CheckDuplicateData(requestModel.ID, requestModel.NationalID, false, fieldType: 1))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_NATIONALID);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.ElectricRankCard, false, fieldType: 2))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_ELECTRICRANKCARD);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.LaborCard) && _userService.CheckDuplicateData(requestModel.ID, requestModel.LaborCard, false, fieldType: 3))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_LABORCARD);
            }

            // Bậc an toàn điện (x/y: x <= y) ví dụ: 1/5 
            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                string[] arrValue = requestModel.ElectricRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.ELECTRICRANK_NOT_VALID);
                }

            }
            // Bậc thợ (x/y: x <= y) ví dụ: 1/7 
            if (!string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                string[] arrValue = requestModel.LaborRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add(CommonConstant.LABORRANK_NOT_VALID);
                }
            }

            return _errorMessages.Count <= 0;
        }

        public ResponseModel<bool> ChangePassword(ChangePasswordModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                var userInfo = _userService.GetCurrentUser(requestModel.UserName);
                // check user exist 
                if (userInfo != null && userInfo.ID > 0)
                {
                    // Kiểm tra độ mạnh của mật khẩu
                    if (!string.IsNullOrWhiteSpace(requestModel.NewPassword))
                    {
                        IsStrongPassword(requestModel.NewPassword);

                        if (_errorMessages.Count > 0)
                        {
                            responseModel.ErrorMessages = _errorMessages;
                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                            return responseModel;
                        }
                    }

                    if (RuntimeManager.ValidatePassword(requestModel.OldPassword, userInfo.Password))
                    {
                        responseModel.Data = _userService.ChangePassword(requestModel);
                    }
                    else
                    {
                        // Tài khoản hoặc mật khẩu chưa chính xác
                        _errorMessages.Add(CommonConstant.OLD_PASSWORD_INVALID);
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    _errorMessages.Add(CommonConstant.ACCOUNT_NOTFOUND);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> LogOut(int userID, string userName, string token)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                responseModel.Data = _userService.LogOut(userID, userName, token);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.OK;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseUser(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _userService.CheckInUseUser(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> MigrationPasswordOldSystem()
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            if (AppSettings.ALLOW_MIGRATE_PASSWORD)
            {
                try
                {
                    responseModel.Data = _userService.MigrationPasswordOldSystem();
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add("Migration password không thành công!");
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(), ex.ToString());
                }
            }
            else
            {
                responseModel.StatusCode = (int)HttpStatusCode.OK;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Thông tin password đã được migrate từ hệ thống cũ!");
            }

            return responseModel;
        }

        public void IsStrongPassword(string password)
        {
            // Password must be at least 10 characters long
            if (password.Length < 10)
            {
                _errorMessages.Add(CommonConstant.PASSWORD_LENGTH);
            }

            // Password must contain at least one uppercase letter
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                _errorMessages.Add(CommonConstant.PASSWORD_UPPERCASE);
            }

            // Password must contain at least one lowercase letter
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                _errorMessages.Add(CommonConstant.PASSWORD_LOWERCASE);
            }

            // Password must contain at least one digit
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                _errorMessages.Add(CommonConstant.PASSWORD_NUMBER);
            }

            // Password can contain special characters, uncomment the line below if you want to enforce
            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                _errorMessages.Add(CommonConstant.PASSWORD_SPECIAL_CHARACTER);
            }
        }

        public void HandleLoginFailed(int userID, bool isLoginFail)
        {
            try
            {
                _userService.HandleLoginFailed(userID, isLoginFail);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                            Environment.NewLine + $"userID: {userID}" + Environment.NewLine + ex.ToString());
            }
        }

        public ResponseModel<bool> CheckExistEmail(int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _userService.CheckExistEmail(userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.INCIDENT);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserBusiness.ToString(),
                    Environment.NewLine + $"userID: {userID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
