using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin cho phép
    /// </summary>
    public class AddAllowedInfoWithoutAcceptorModel
    {
        /// <summary>
        /// Danh sách thông tin cho phép
        /// </summary>
        public List<AddAllowedInfoDetailModel> AllowedInfos { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Người chỉ huy. đối với phiếu thủy cơ nhiệt
        /// </summary>
        public LeadershipUserSigningModel Commander { get; set; }

        /// <summary>
        /// Người chỉ huy. đối với phiếu thủy cơ nhiệt
        /// </summary>
        public LeadershipUserSigningModel Allower { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }        
    }
}
