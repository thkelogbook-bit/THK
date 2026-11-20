namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các giá trị thông số vận hành của mỗi khung giờ
    /// </summary>
    public class ParameterValueModel
    {
        /// <summary>
        /// ID chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// ID thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Khung giờ ghi thông số
        /// </summary>
        public string OpeartionTimeLineName { get; set; }

        /// <summary>
        /// Giá trị của thông số vận hành
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// Màu cảnh báo
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Mã màu
        /// </summary>
        public string ColorCode { get; set; }
    }
}
