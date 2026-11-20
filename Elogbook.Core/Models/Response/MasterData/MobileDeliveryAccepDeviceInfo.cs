using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Table lưu dữ liệu giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
    /// </summary>
    public class MobileDeliveryAccepDeviceInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// True nếu giao nhận, nghiệm thu trước. False nếu giao nhận, nghiệm thu sau
        /// </summary>
        public bool IsBeforeDelivery { get; set; }

        /// <summary>
        /// Thời gian
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }
    }
}
