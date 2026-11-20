using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// các tham số Nhóm thông số vận hành
    /// </summary>
    public class ParametersChartDetailModel
    {
        /// <summary>
        /// ID thông số
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Mã danh mục thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public string Ngay { get; set; }

        /// <summary>
        /// Giờ chu kỳ ghi thông số
        /// </summary>
        public int Gio { get; set; }

        /// <summary>
        /// Giá trị giờ-ngày
        /// </summary>
        public string XValue { get; set; }

        /// <summary>
        /// Giá trị thông số đã ghi
        /// </summary>
        public string YValue { get; set; }

        /// <summary>
        /// Ngày chu kỳ ghi nhật ký
        /// </summary>
        public DateTime CycleDate { get; set; }
    }
}
