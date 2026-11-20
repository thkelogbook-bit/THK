using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Danh sách Thông tin bất thường sự cố
    /// </summary>
    public class AbnormalIncidentDetailSyncDataModel
    {
        /// <summary>
        /// Thông tin xử lý bất thường sự cố
        /// </summary>
        public List<AbnormalIncidentDetailSyncModel> AbnormalIncidentSyncDetails { get; set; }

        /// <summary>
        /// Thông tin bất thường sự cố
        /// </summary>
        public List<AbnormalIncidentSyncModel> AbnormalIncidentSync { get; set; }
    }
}
