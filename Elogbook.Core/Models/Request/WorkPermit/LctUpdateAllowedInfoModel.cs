using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Item Update Thông tin cho phép
    /// </summary>
    public class LctUpdateAllowedInfoModel
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Người chỉ huy. đối với lệnh thủy cơ nhiệt
        /// </summary>
        public LctLeadershipUserSigningModel Commander { get; set; }

        /// <summary>
        /// Người chỉ huy. đối với lệnh thủy cơ nhiệt
        /// </summary>
        public LctLeadershipUserSigningModel Allower { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
