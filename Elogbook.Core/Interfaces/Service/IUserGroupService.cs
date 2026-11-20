using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.User;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức cho chức năng người dùng
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống
        /// </summary>
        /// <returns></returns>
        List<EmployeeInfoModel> GetAllUsers(bool isExternal);

        /// <summary>
        /// Kiểm tra tài khoản người dùng đã tồn tại
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userId"></param>
        /// <param name="isExternal"></param>
        /// <returns></returns>
        bool CheckDuplicateUser(int iD, int userId, bool isExternal);

        /// <summary>
        /// Kiểm tra nhóm người dùng đã tồn tại
        /// </summary>
        /// <param name="factoryId"></param>
        /// <param name="userGroupName"></param>
        /// <returns></returns>
        bool CheckDuplicateUserGroup(int factoryId, string userGroupName);

        /// <summary>
        /// Lấy thông tin nhóm người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserGroupModel GetUserGroupById(int id);

        /// <summary>
        /// Tìm kiếm nhóm người dùng theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetUserGroups(SearchUserGroupModel requestModel);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool AddUserGroup(AddEditUserGroupModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateUserGroup(AddEditUserGroupModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteUserGroup(int iD, int userID);
    }
}
