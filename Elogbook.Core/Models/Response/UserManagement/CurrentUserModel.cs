using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.User
{
    /// <summary>
    /// Thông tin của người dùng đăng nhập
    /// </summary>
    public class CurrentUserModel : UserModel
    {
        /// <summary>
        /// Token để truy cập vào api
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Thời gian hết hạn token(tính bằng số phút)
        /// </summary>
        public int TokenExpireTime { get; set; }

        /// <summary>
        /// Danh sách quyền người dùng
        /// </summary>
        public List<UserRightModel> UsersRight { get; set; }

        /// <summary>
        /// Giới hạn quyền truy cập
        /// </summary>
        public bool IsLimited { get; set; }
    }
}
