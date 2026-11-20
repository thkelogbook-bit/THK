using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin đồng bộ bất thường sự cố
    /// </summary>
    public class AbnormalIncidentDetailSyncDataResultModel
    {
        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public List<MobileAbnormalIncidentInfo> AbnormalIncident { get; set; }

        /// <summary>
        /// Chi tiết bất thường sự cố
        /// </summary>
        public List<MobileAbnormalIncidentDetailInfo> AbnormalIncidentDetail { get; set; }
    }
}
