using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin cho phép
    /// </summary>
    public class LctAddAllowedInfoWithoutAcceptorModel
    {
        /// <summary>
        /// Danh sách thông tin cho phép
        /// </summary>
        public List<LctAddAllowedInfoDetailModel> AllowedInfos { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

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
