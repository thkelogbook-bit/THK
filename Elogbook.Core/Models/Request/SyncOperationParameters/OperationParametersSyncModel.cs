using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>Các tham số của Thông số vận hành </summary>
    public class OperationParametersSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Key Mapping
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Id của thông số vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên thông số vận hành
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// ID Thông số (Danh mục)
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// ID Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Id Khung giờ
        /// </summary>
        public int? OperationTimeLineID { get; set; }

        /// <summary>
        /// Id Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ID Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Id Chu kỳ
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
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Tạo mới
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật dữ liệu
        /// </summary>
        public DateTime? CheckedDate { get; set; }
    }
}
