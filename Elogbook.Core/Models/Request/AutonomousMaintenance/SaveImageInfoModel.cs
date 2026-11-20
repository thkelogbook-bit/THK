using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Hình ảnh trước or sau thực hiện AM
    /// </summary>
    public class SaveImageInfoModel
    {
        /// <summary>
        /// ID AM
        /// </summary>
        public int AmID { get; set; }

        /// <summary>
        /// Danh sách iamge trước thực hiện
        /// </summary>
        public string BeforeImageAttIds { get; set; }

        /// <summary>
        /// Danh sách iamge sau thực hiện
        /// </summary>
        public string AfterImageAttIds { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
