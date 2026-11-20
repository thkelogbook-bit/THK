using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết TSVH
    /// </summary>
    public class ParametersChartModel
    {
        /// <summary>
        /// Mã danh mục thông số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Mã KKS
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string UnitMeasure { get; set; }

        /// <summary>
        /// Cương vị - Nhà máy
        /// </summary>
        public string OperationPosition_Factory_Name { get; set; }

        /// <summary>
        /// Giá trị min của trục hoành
        /// </summary>
        public string MinX { get; set; }

        /// <summary>
        /// Thời gian nhỏ nhất ghi thông số
        /// </summary>
        public DateTime MinDateX { get; set; }

        /// <summary>
        /// Giá trị max của trục hoành
        /// </summary>
        public string MaxX { get; set; }

        /// <summary>
        /// Thời gian lớn nhất ghi thông số
        /// </summary>
        public DateTime MaxDateX { get; set; }

        /// <summary>
        /// Giá trị min của trục tung
        /// </summary>
        public decimal MinY { get; set; }

        /// <summary>
        /// Giá trị max của trục tung
        /// </summary>
        public decimal MaxY { get; set; }

        /// <summary>
        /// Dữ liệu ghi TSVH
        /// </summary>
        public List<ParametersChartDetailModel> ParametersChartDetails { get; set; }

        /// <summary>
        /// Label hiển thị trên trục X
        /// </summary>
        public List<string> Xaxis { get; set; }
    }
}
