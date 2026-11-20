using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức quản lý người ngoài hệ thống
    /// </summary>
    public interface IGuestUserBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả người ngoài hệ thống theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetGuestUsers(SearchGuestUserModel requestModel);

        /// <summary>
        /// Tìm kiếm người ngoài hệ thống theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<GuestUserModel> GetGuestUserById(int id);

        /// <summary>
        /// Thêm mới người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<GuestUserModel> AddGuestUser(AddEditGuestUserModel requestModel);

        /// <summary>
        /// Cập nhật người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateGuestUser(AddEditGuestUserModel requestModel);

        /// <summary>
        /// Xóa người ngoài hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteGuestUser(int id, int userID);

        /// <summary>
        /// Kiểm tra người ngoài hệ thống đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseGuestUser(int id);
    }
}
