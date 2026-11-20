
namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// User token
    /// </summary>
    public class UserTokenModel
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Họ và Tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Thời gian hết hạn token(tính bằng số phút)
        /// </summary>
        public int TokenExpireTime { get; set; }
    }
}
