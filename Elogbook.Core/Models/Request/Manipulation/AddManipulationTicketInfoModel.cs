using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Thông tin cấp phiếu
    /// </summary>
    public class AddManipulationTicketInfoModel
    {
        /// <summary>
        /// mã số phiếu thao tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// mã số đăng ký phiếu thao tác
        /// </summary>
        public int RegistrationManipulationTicketID { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime ApprovedDate { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách giao nhận nghiệm thu đường dây,  thiết bị điện trước khi thao tác
        /// </summary>
        public List<AddDeliveryAccepDeviceModel> DeliveryAccepDeviceModels { get; set; }
    }
}
