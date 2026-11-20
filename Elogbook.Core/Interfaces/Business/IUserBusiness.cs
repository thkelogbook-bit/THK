using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.User;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho chức năng người dùng
    /// </summary>
    public interface IUserBusiness
    {
        /// <summary>
        /// Trả về danh sách người dùng theo các tiêu chí tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetUsers(SearchUserModel requestModel);

        /// <summary>
        /// Trả về người dùng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<UserModel> GetUserById(int id);

        /// <summary>
        /// Trả về người dùng theo tài khoản người dùng
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        CurrentUserModel GetCurrentUser(string username);

        /// <summary>
        /// Thêm mới người dùng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddUser(AddEditUserModel requestModel);

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng trên web
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateUserProfileWeb(UpdateUserProfileWebModel requestModel);

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng trên mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateUserProfileMobile(UpdateUserProfileMobileModel requestModel);

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateUser(AddEditUserModel requestModel);

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteUser(int id, int userID);

        /// <summary>
        /// Kiểm tra người dùng đã tồn tại hay chưa
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckExistUserByUserName(string userName);

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> ChangePassword(ChangePasswordModel requestModel);

        /// <summary>
        /// Đăng xuất tài khoản người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        ResponseModel<bool> LogOut(int userID, string userName, string token);

        /// <summary>
        /// Kiểm tra User đang được sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseUser(int id);

        /// <summary>
        /// Phương thức migrate password tất cả người dùng từ hệ thống cũ
        /// </summary>
        /// <returns></returns>
        ResponseModel<bool> MigrationPasswordOldSystem();

        /// <summary>
        /// Xử lý đăng nhập người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="isLoginFail"></param>
        void HandleLoginFailed(int userID, bool isLoginFail);

        /// <summary>
        /// Kiểm tra người dùng có tồn tại email hay không
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckExistEmail(int userID);
    }
}
