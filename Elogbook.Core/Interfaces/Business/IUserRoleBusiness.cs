using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý vai trò
    /// </summary>
    public interface IUserRoleBusiness
    {
        /// <summary>
        /// Dánh sách quyền của hệ thống
        /// </summary>
        /// <returns></returns>
        List<UserPermissionModel> GetPermissions();

        /// <summary>
        /// Tìm kiếm danh sách vai trò theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetUserRoles(RoleRequestModel requestModel);

        /// <summary>
        /// Tìm kiếm vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<UserRoleModel> GetUserRoleById(int id);

        /// <summary>
        /// Thêm vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddUserRole(AddEditUserRoleModel requestModel);

        /// <summary>
        /// Cập nhật vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateUserRole(AddEditUserRoleModel requestModel);

        /// <summary>
        /// Xoá vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteUserRole(int id, int userID);

        /// <summary>
        /// Kiểm tra Vai trò đã sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseUserRole(int id);
    }
}
