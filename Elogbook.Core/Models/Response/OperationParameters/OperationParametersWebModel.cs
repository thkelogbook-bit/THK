using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Dữ liệu Thông số vận hành hiển thị lên Web
    /// </summary>
    public class OperationParametersWebModel
    {
        /// <summary>
        /// ID thông số
        /// </summary>
        public int ParamaterID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string UnitMeasure { get; set; }

        /// <summary>
        /// Mã KKS
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Giới hạn giá trị (từ)
        /// </summary>
        public float? FromRange { get; set; }

        /// <summary>
        /// Giới hạn giá trị (đến)
        /// </summary>
        public float? ToRange { get; set; }

        /// <summary>
        /// Giới hạn nhỏ nhất
        /// </summary>
        public string MinValue { get; set; }

        /// <summary>
        /// Giới hạn lớn nhất
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        /// Danh sách khung giờ và giá trị thông số
        /// </summary>
        public List<ParameterValueModel> ParameterValues { get; set; }

        /// <summary>
        /// Loại thông số (true: số, false: text)
        /// </summary>
        public bool IsNumberType { get; set; }
    }
}
