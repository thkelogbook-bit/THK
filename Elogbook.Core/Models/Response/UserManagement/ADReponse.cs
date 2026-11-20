namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin trả về khi kiểm tra người dùng AD
    /// </summary>
    public class ADReponse
    {
        /// <summary>
        /// Thông tin của người dùng
        /// </summary>
        public ADUserInfo UserInfo { get; set; } = null;

        /// <summary>
        /// Mã lỗi nếu có
        /// </summary>
        public int? ErrorCode { get; set; } = null;

        /// <summary>
        /// Nội dung lỗi nếu có
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
