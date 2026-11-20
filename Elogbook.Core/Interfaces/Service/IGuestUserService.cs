using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức quản lý người ngoài hệ thống
    /// </summary>
    public interface IGuestUserService
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
        /// <param name="isHasAttachent"></param>
        /// <returns></returns>
        GuestUserModel GetGuestUserById(int id, bool isHasAttachent = true);

        /// <summary>
        /// Thêm mới người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        GuestUserModel AddGuestUser(AddEditGuestUserModel requestModel);

        /// <summary>
        /// Cập nhật người ngoài hệ thống
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateGuestUser(AddEditGuestUserModel requestModel);

        /// <summary>
        /// Xóa người ngoài hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteGuestUser(int id, int userID);

        /// <summary>
        /// Kiểm tra người ngoài hệ thống đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseGuestUser(int id);

        /// <summary>
        /// Kiểm tra tồn tại dữ liệu
        /// </summary>
        /// <param name="guestUserID"></param>
        /// <param name="description"></param>
        /// <param name="isAddMode"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int guestUserID, string description, bool isAddMode, int fieldType);
    }
}
