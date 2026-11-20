using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tham số hoàn thành phiếu thao tác
    /// </summary>
    public class CompleteManipulationModel
    {
        /// <summary>
        /// ID Phiếu thao tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 5. Điều kiện cần có thể thực hiện và 6. Lưu ý
        /// </summary>
        public List<AddDeliveryAccepDeviceModel> DeliveryAccepDevices { get; set; }

        /// <summary>
        /// Danh sách file sơ đồ
        /// </summary>
        public List<string> MapAttachmentIDs { get; set; }

        /// <summary>
        /// Thông tin người ký
        /// </summary>
        public UserInfoInTicketModel SignerInfo { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
