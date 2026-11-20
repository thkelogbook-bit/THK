using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một Từ chối cấp lệnh
    /// </summary>
    public class LctAddTicketStatusHistoryModel
    {
        /// <summary>
        /// ID lệnh đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int StatusTicket { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public List<string> AttachmentsID { get; set; }

        /// <summary>
        /// Người từ chối
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
