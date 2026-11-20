using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class AddLctOrderExecutionMobileModel
    {
        /// <summary>
        /// lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy trực tiếp
        /// </summary>
        public LctLeadershipUserSigningModel Commander { get; set; }

        /// <summary>
        /// Chữ ký giám sát
        /// </summary>
        public LctLeadershipUserSigningModel Supervisor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AddEditWorkProcessSafetyMobileModel> WorkProcessSafety { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
