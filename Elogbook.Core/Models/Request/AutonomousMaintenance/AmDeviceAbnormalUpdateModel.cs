using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cập nhật trạng thái bất thường thiết bị
    /// </summary>
    public class AmDeviceAbnormalUpdateModel
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public int? DeviceAbnormalStatusID { get; set; }

        /// <summary>
        /// Mô tả bất thường thiết bị
        /// </summary>
        public string DeviceAbnormalDescription { get; set; }

        /// <summary>
        /// ID Các file bất thường thiết bị, Hình ảnh or Video
        /// </summary>
        public string DeviceAbnormalAttachments { get; set; }

        /// <summary>
        /// ID phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Có bất thường
        /// </summary>
        public bool IsAbnormal { get; set; } = false;

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
