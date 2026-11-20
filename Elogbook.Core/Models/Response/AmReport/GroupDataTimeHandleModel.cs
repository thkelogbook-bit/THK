namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// khai báo model để group dữ liệu tổng quan
    /// </summary>
    public class GroupDataGeneralTimeHandleModel
    {
        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public ValueLabelCheckedModel Emergency { get; set; }

        /// <summary>
        /// Tổng số bất thường đã hoàn thành sữa chữa
        /// </summary>
        public int TotalEmergency { get; set; } = 0;

        /// <summary>
        /// Tổng số ngày xử lý
        /// </summary>
        public int TotalDate { get; set; } = 0;

        /// <summary>
        /// Thời gian trung bình
        /// </summary>
        public double AverageTime { get; set; } = 0;

        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
    }
}
