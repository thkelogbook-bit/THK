using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin cho phép
    /// </summary>
    public class AddAllowedInfoModel
    {
        /// <summary>
        /// Danh sách thông tin cho phép
        /// </summary>
        public List<AddAllowedInfoDetailModel> AllowedInfos { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public AddLeadershipUserModel Acceptor { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        public string TimeCommanderSigned { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        
    }
}
