namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin các cương vị trong ca
    /// </summary>
    public class AddEditAlertDetailModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Loại cảnh báo
        /// </summary>
        public string Alert { get; set; }

        /// <summary>
        /// Toán tử
        /// </summary>
        public string Operator  { get; set; }

        /// <summary>
        /// Giá trị
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Màu
        /// </summary>
        public string Color { get; set; }
    }
}
