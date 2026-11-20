using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Hủy phiếu thao tác
    /// </summary>
    public class CancelTicketSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// Mã thông tin yêu cầu
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
