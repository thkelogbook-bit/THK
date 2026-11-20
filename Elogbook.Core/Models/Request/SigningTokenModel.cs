
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin chữ ký số
    /// </summary>
    public class SigningTokenModel
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// chức năng
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// mã PCT
        /// </summary>
        public int WorkTicketId { get; set; }

        /// <summary>
        /// chữ ký
        /// </summary>
        public SignatureModel Signature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
