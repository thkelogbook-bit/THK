namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tham số để thêm 1 điều kiện để thực hiện
    /// </summary>
    public class AddDeliveryAccepDeviceModel
    {
        /// <summary>
        /// Thời gian
        /// </summary>
        public string DeliveryTime { get; set; }

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
