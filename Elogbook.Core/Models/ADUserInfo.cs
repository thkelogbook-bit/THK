namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin người dùng AD
    /// </summary>
    public class ADUserInfo
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }
    }
}
