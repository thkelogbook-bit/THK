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
    /// Quản lý nhóm người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/userGroup")]
    public class UserGroupController : BaseApiController
    {
        private readonly IUserGroupBusiness _userGroupBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userGroupBusiness"></param>
        public UserGroupController(IUserGroupBusiness userGroupBusiness)
        {
            _userGroupBusiness = userGroupBusiness;
        }

        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống
        /// </summary>
        /// <param name="isExternal">True: Người ngoài hệ thống, False: Người nội bộ</param>
        /// <returns></returns>
        [Route("getAllUsers")]
        [HttpGet]
        [Permission(EnumUserPermission.ADD_NND, EnumUserPermission.EDIT_NND)]
        public List<EmployeeInfoModel> GetAllUsers(bool isExternal = false)
        {
            return _userGroupBusiness.GetAllUsers(isExternal);
        }

        /// <summary>
        /// Tìm kiếm một nhóm người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_NND)]
        public ResponseModel<UserGroupModel> GetUserGroupById([FromUri] int id)
        {
            return _userGroupBusiness.GetUserGroupById(id);
        }

        /// <summary>
        /// Tìm kiến nhân viên theo điều kiện
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_NND)]
        public ResponseBaseModel GetUserGroups([FromBody] SearchUserGroupModel requestBody)
        {
            return _userGroupBusiness.GetUserGroups(requestBody);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_NND)]
        public ResponseModel<bool> AddUserGroup([FromBody] AddEditUserGroupModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userGroupBusiness.AddUserGroup(requestModel);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_NND)]
        public ResponseModel<bool> UpdateUserGroup([FromBody] AddEditUserGroupModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _userGroupBusiness.UpdateUserGroup(requestModel);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_NND)]
        public ResponseModel<bool> DeleteUser([FromUri] int id)
        {
            return _userGroupBusiness.DeleteUserGroup(id, CurrentUser.ID);
        }
    }
}
