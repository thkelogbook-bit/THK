using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý vai trò
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/userRole")]
    public class UserRoleController : BaseApiController
    {
        private readonly IUserRoleBusiness _userRoleBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRoleBusiness"></param>
        public UserRoleController(IUserRoleBusiness userRoleBusiness)
        {
            _userRoleBusiness = userRoleBusiness;
        }

        /// <summary>
        /// Danh sách quyền hệ thống
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_VT
        [Route("getPermissions")]
        [HttpGet]
        public List<UserPermissionModel> GetPermissions()
        {
            return _userRoleBusiness.GetPermissions();
        }

        /// <summary>
        /// Tìm kiếm vai trò theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_VT
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_VT)]
        public ResponseBaseModel GetUserRoles([FromBody] RoleRequestModel requestModel)
        {
            return _userRoleBusiness.GetUserRoles(requestModel);
        }

        /// <summary>
        /// Tìm kiếm vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_VT
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_VT)]
        public ResponseModel<UserRoleModel> GetUserRoleById([FromUri] int id)
        {
            return _userRoleBusiness.GetUserRoleById(id);
        }

        /// <summary>
        /// Thêm vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_VT)]
        public ResponseModel<int> AddUserRole([FromBody] AddEditUserRoleModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userRoleBusiness.AddUserRole(requestModel);
        }

        /// <summary>
        /// Cập nhật thông tin vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_VT)]
        public ResponseModel<bool> UpdateUserRole([FromBody] AddEditUserRoleModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userRoleBusiness.UpdateUserRole(requestModel);
        }

        /// <summary>
        /// Kiểm tra vai trò đã sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_VT)]
        public ResponseModel<bool> CheckInUseUserRole([FromUri] int id)
        {
            return _userRoleBusiness.CheckInUseUserRole(id);
        }

        /// <summary>
        /// Xoá vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_VT)]
        public ResponseModel<bool> DeleteUserRole([FromUri] int id)
        {
            return _userRoleBusiness.DeleteUserRole(id, CurrentUser.ID);
        }
    }
}