using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Chi tiết thông tin cho phép
    /// </summary>
    public class AddAllowedInfoDetailModel
    {
        /// <summary>
        /// Mã
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nội dung cho phép
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Đã được đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public List<string> Attachments { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
