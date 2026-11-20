using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// ManipulationTicketSyncDataResultModel
    /// </summary>
    public class ManipulationTicketSyncDataResultModel
    {
        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public List<TicketDetailModel> TicketDetailModels { get; set; }
    }
}
