using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Hủy phiếu thao tác
    /// </summary>
    public class CancelTicketModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int IdTicket { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
