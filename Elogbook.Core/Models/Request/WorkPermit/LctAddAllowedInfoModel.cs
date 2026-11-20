using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin cho phép
    /// </summary>
    public class LctAddAllowedInfoModel
    {
        /// <summary>
        /// Danh sách thông tin cho phép
        /// </summary>
        public List<LctAddAllowedInfoDetailModel> AllowedInfos { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LctAddLeadershipUserModel Acceptor { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        public string TimeCommanderSigned { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
