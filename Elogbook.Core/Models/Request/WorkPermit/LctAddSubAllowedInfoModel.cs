using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để cập nhật một biện pháp lúc cho phép
    /// </summary>
    public class LctAddSubAllowedInfoModel
    {
        /// <summary>
        /// Số định danh
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nội dung cho phép
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public List<string> Attachments { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
