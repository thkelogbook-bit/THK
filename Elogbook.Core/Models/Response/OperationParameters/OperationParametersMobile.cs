namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông số vận hành (mobile)
    /// </summary>
    public class OperationParametersMobile
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Giá trị thông số đã ghi
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Giá trị thông số đã ghi
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string UnitMeasure { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Màu
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Mã màu
        /// </summary>
        public string ColorCode { get; set; }
    }
}
