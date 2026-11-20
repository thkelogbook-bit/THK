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
    public class UserRoleBusiness : IUserRoleBusiness
    {
        private readonly IUserRoleService _rolesService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UserRoleBusiness(IUserRoleService rolesService)
        {
            _rolesService = rolesService;
        }

        public List<UserPermissionModel> GetPermissions()
        {
            try
            {
                return _rolesService.GePermissions();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(), ex.ToString());
                return new List<UserPermissionModel>();
            }
        }

        /// <summary>
        /// Tìm tất cả vai trò của người dùng theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetUserRoles(RoleRequestModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _rolesService.GetUserRoles(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<UserRoleModel> GetUserRoleById(int id)
        {
            var responseModel = new ResponseModel<UserRoleModel>();

            try
            {
                responseModel.Data = _rolesService.GetUserRoleById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Thêm mới Vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddUserRole(AddEditUserRoleModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    if (!_rolesService.CheckDuplicateData(0, requestModel.RoleName, true))
                    {
                        responseModel.Data = _rolesService.AddUserRole(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_ROLE_NAME);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật Vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateUserRole(AddEditUserRoleModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    if (!_rolesService.CheckDuplicateData(requestModel.ID, requestModel.RoleName, false))
                    {
                        responseModel.Data = _rolesService.UpdateUserRole(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_ROLE_NAME);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xoá vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteUserRole(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _rolesService.DeleteUserRole(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseUserRole(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _rolesService.CheckInUseUserRole(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.UserRoleBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Phân xưởng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditUserRoleModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.RoleName))
            {
                _errorMessages.Add(CommonConstant.ROLE_NAME_REQUIRED);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Permissions))
            {
                var isExistPermission = _rolesService.CheckPermission(requestModel.Permissions);

                if (!isExistPermission)
                {
                    _errorMessages.Add("Quyền không hợp lệ, vui lòng kiểm tra lại");
                }
            }

            return _errorMessages.Count <= 0;
        }
    }
}
