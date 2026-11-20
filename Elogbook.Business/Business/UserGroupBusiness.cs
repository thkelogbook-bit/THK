using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class UserGroupBusiness : IUserGroupBusiness
    {
        private readonly IUserGroupService _userGroupService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UserGroupBusiness(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống
        /// </summary>
        /// <returns></returns>
        public List<EmployeeInfoModel> GetAllUsers(bool isExternal)
        {
            var responseBaseModel = new ResponseBaseModel();

            try
            {
                return _userGroupService.GetAllUsers(isExternal);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(), ex.ToString());
                return new List<EmployeeInfoModel>();
            }
        }

        /// <summary>
        /// Lấy thông tin nhóm người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<UserGroupModel> GetUserGroupById(int id)
        {
            var responseModel = new ResponseModel<UserGroupModel>();

            try
            {
                responseModel.Data = _userGroupService.GetUserGroupById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm nhóm người dùng theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetUserGroups(SearchUserGroupModel requestModel)
        {
            var responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _userGroupService.GetUserGroups(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> AddUserGroup(AddEditUserGroupModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel == null)
                {
                    responseModel.ErrorMessages.Add("Dữ liệu đangg bị trống");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // Check input
                if (!IsValidData(requestModel, true))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _userGroupService.AddUserGroup(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateUserGroup(AddEditUserGroupModel requestModel)
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
                    responseModel.Data = _userGroupService.UpdateUserGroup(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteUserGroup(int iD, int userID)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel.Data = _userGroupService.DeleteUserGroup(iD, userID);
                if (!responseModel.Data)
                {
                    responseModel.StatusCode = -1;
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserGroupBusiness.ToString(),
                    Environment.NewLine + $"ID: {iD} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditUserGroupModel requestModel, bool isInsert)
        {
            if (isInsert)
            {
                // Kiểm tra nhà máy
                if (requestModel.FactoryID < 1)
                {
                    _errorMessages.Add(CommonConstant.FACTORY_COMPANY_REQUIRED);
                }

                // Kiểm tra nhóm người dùng
                if (isInsert && string.IsNullOrWhiteSpace(requestModel.UserGroupName))
                {
                    _errorMessages.Add("Tên nhóm người dùng là bắt buộc");
                }

                if (requestModel.FactoryID > 0 && !string.IsNullOrWhiteSpace(requestModel.UserGroupName))
                {
                    if (_userGroupService.CheckDuplicateUserGroup(requestModel.FactoryID, requestModel.UserGroupName))
                    {
                        _errorMessages.Add("Đã tồn tại nhóm người dùng trong nhà máy");
                    }
                }
            }
            else
            {
                // Id nhóm người dùng
                if (requestModel.ID < 1)
                {
                    _errorMessages.Add("Id nhóm người dùng cần cập nhật là thông tin bắt buộc");
                }
            }

            if (requestModel.UserAccounts.Count > 0)
            {
                List<int> userInternal = new List<int>();
                List<int> userExternal = new List<int>();

                foreach (var item in requestModel.UserAccounts)
                {
                    // Kiểm tra id tài khoản người dùng
                    if (item.UserID < 1)
                    {
                        _errorMessages.Add("ID tài khoản người dùng là bắt buộc.[" + item.FullName + "]");
                    }
                    else
                    {
                        // Loại tài khoản người dùng là bắt buộc
                        if (item.IsExternal == null)
                        {
                            _errorMessages.Add("Loại tài khoản người dùng là bắt buộc.[" + item.FullName + "]");
                        }
                        else
                        {
                            // người trong hệ thống
                            if (item.IsExternal != true)
                            {
                                if (!userInternal.Contains(item.UserID))
                                {
                                    // trường hợp thêm mới
                                    if (isInsert)
                                    {
                                        userInternal.Add(item.UserID);
                                    }
                                    // trường hợp cập nhật
                                    else
                                    {
                                        if (item.IsInsert)
                                        {
                                            if (!_userGroupService.CheckDuplicateUser(requestModel.ID, item.UserID, false))
                                            {
                                                userInternal.Add(item.UserID);
                                            }
                                            else
                                            {
                                                _errorMessages.Add("Tài khoản người dùng đã tồn tại.[" + item.FullName + "]");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _errorMessages.Add("Tài khoản người dùng đang bị trùng lặp.[" + item.FullName + "]");
                                }
                            }

                            // người ngoài hệ thống
                            if (item.IsExternal == true)
                            {
                                if (!userExternal.Contains(item.UserID))
                                {
                                    if (isInsert)
                                    {
                                        userExternal.Add(item.UserID);
                                    }
                                    else
                                    {
                                        if (item.IsInsert)
                                        {
                                            if (!_userGroupService.CheckDuplicateUser(requestModel.ID, item.UserID, true))
                                            {
                                                userExternal.Add(item.UserID);
                                            }
                                            else
                                            {
                                                _errorMessages.Add("Tài khoản người dùng đã tồn tại.[" + item.FullName + "]");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _errorMessages.Add("Tài khoản người dùng đang bị trùng lặp.[" + item.FullName + "]");
                                }
                            }
                        }
                    }
                }
            }

            return _errorMessages.Count <= 0;
        }
    }
}
