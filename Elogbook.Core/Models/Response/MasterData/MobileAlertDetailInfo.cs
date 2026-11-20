namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin setting cho thông số
    /// </summary>
    public class MobileAlertDetailInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Thông số
        /// </summary>
        public int? ParameterID { get; set; }

        /// <summary>
        /// Alert
        /// </summary>
        public string Alert { get; set; }

        /// <summary>
        /// Toán tử
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Giá trị
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Màu hiển thị
        /// </summary>
        public string Color { get; set; }
    }
}
