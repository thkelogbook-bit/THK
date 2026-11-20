using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để cập nhật chi tiết phiếu công tác
    /// </summary>
    public class AddDetailTicketMobileModel
    {
        /// <summary>
        /// Danh sách hình ảnh công tác
        /// </summary>
        public List<string> WorkPictures { get; set; }

        /// <summary>
        /// Kết quả công tác hàng ngày
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }
    }
}
