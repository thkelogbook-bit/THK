using Elogbook.Core.Models.Request;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kết thúc phiếu thao tác
    /// </summary>
    public class CloseTicketModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int IdTicket { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Người dùng ký
        /// </summary>
        public List<UserInfoInTicketModel> Supervisors { get; set; }

        /// <summary>
        /// Người thao tác 1
        /// </summary>
        public List<UserInfoInTicketModel> Workers { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
