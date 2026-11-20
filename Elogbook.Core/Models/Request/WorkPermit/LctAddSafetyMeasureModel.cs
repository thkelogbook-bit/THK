using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm một biện pháp
    /// </summary>
    public class LctAddSafetyMeasureModel
    {
        /// <summary>
        /// Nội dung biện pháp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nội dung biện pháp
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Đã được đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? Position { get; set; }

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
