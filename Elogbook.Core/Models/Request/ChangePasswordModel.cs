using Newtonsoft.Json;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin change mật khẩu
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Mật khẩu cũ
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Tài khoản người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Mã người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
