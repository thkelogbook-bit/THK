using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.User;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho chức năng người dùng
    /// </summary>
    public interface IUserGroupBusiness
    {
        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống
        /// </summary>
        /// <returns></returns>
        List<EmployeeInfoModel> GetAllUsers(bool isExternal);

        /// <summary>
        /// Lấy thông tin nhóm người
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<UserGroupModel> GetUserGroupById(int id);

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
        ResponseModel<bool> AddUserGroup(AddEditUserGroupModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateUserGroup(AddEditUserGroupModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteUserGroup(int iD, int userID);
    }
}
