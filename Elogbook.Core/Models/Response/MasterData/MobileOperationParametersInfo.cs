using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// MobileOperationParametersInfo
    /// </summary>
    public class MobileOperationParametersInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Thông số ID
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int? OperationTimeLineID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Giá trị được ghi
        /// </summary>
        public string CurrentValue { get; set; }

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
        /// Giới trị trước đó
        /// </summary>
        public string PreviousValue { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Đã được ghi
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Danh sách cảnh báo
        /// </summary>
        public string AlertDetails { get; set; }

        /// <summary>
        /// Key mapping
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Ngày cập nhật dữ liệu
        /// </summary>
        public DateTime? CheckedDate { get; set; }
    }
}
