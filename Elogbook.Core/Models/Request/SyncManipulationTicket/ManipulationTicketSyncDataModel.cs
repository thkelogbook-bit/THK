using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Danh sách Thông tin PTT
    /// </summary>
    public class ManipulationTicketSyncDataModel
    {
        /// <summary>
        /// Thông tin tồn tại trong ca
        /// </summary>
        public List<ManipulationTicketSyncModel> ManipulationTicketSyncModels { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
