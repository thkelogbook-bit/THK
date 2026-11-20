using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông số cần ghi
    /// </summary>
    public class OperationParametersModel
    {
        /// <summary>
        /// ID thông số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã thông số trong danh mục
        /// </summary>
        [JsonIgnore]
        public int ParameterID { get; set; }

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
        /// Setting
        /// </summary>
        public string Setting { get; set; }

        /// <summary>
        /// Giá trị chu kỳ trước
        /// </summary>
        public string PreviousValue { get; set; }

        /// <summary>
        /// Giá trị cần ghi
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Giới hạn giá trị (từ)
        /// </summary>
        public float? FromRange { get; set; }

        /// <summary>
        /// Giới hạn giá trị (đến)
        /// </summary>
        public float? ToRange { get; set; }

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        public bool IsNumberType { get; set; }

        /// <summary>
        /// Thuộc nhóm thông số
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int? OprerationTimeLineID { get; set; }

        /// <summary>
        /// Xác định thông số đã ghi dữ liệu hay chưa
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Danh sách thông tin cảnh báo
        /// </summary>
        public string AlertDetails { get; set; }

        /// <summary>
        /// Ngày cập nhật dữ liệu
        /// </summary>
        public DateTime? CheckedDate { get; set; }

        /// <summary>
        /// Số dòng dữ liệu map với điều kiện tìm kiếm
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
