using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin chữ ký
    /// </summary>
    public class SignatureModel
    {
        /// <summary>
        /// Mã chữ ký
        /// </summary>
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string SignatureContent { get; set; }

        /// <summary>
        /// Loại chữ ký
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
