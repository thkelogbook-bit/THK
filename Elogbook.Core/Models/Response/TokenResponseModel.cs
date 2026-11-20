namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin token của người dùng
    /// </summary>
    public class TokenResponseModel
    {
        /// <summary>
        /// Token để truy cập vào api
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Thời gian hết hạn token(tính bằng số phút)
        /// </summary>
        public int TokenExpireTime { get; set; }
    }
}
