using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý vai trò
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// Dánh sách quyền của hệ thống
        /// </summary>
        /// <returns></returns>
        List<UserPermissionModel> GePermissions();

        /// <summary>
        /// Tìm tất cả vai trò theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetUserRoles(RoleRequestModel requestModel);

        /// <summary>
        /// Tìm kiếm vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserRoleModel GetUserRoleById(int id);

        /// <summary>
        /// Thêm mới Vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddUserRole(AddEditUserRoleModel requestModel);

        /// <summary>
        /// Cập nhật vai trò
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateUserRole(AddEditUserRoleModel requestModel);

        /// <summary>
        /// Xoá vai trò theo id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteUserRole(int id, int userID);

        /// <summary>
        /// Kiểm tra Vai trò đã sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseUserRole(int id);

        /// <summary>
        /// Kiểm tra tồn tại Vai trò khi thêm mới/cập nhật
        /// </summary>
        /// <param name="userRoleID"></param>
        /// <param name="roleName"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int userRoleID, string roleName, bool isAddMode);

        /// <summary>
        /// Check quyền tồn tại
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool CheckPermission(string permissions);
    }
}
