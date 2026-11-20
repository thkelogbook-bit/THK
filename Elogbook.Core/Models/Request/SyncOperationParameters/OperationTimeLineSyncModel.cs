using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>Các tham số của Khung giờ thông số vận hành </summary>
    public class OperationTimeLineSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã khung giờ
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Key Mapping ngày-nhà máy-cương vị-chu kỳ Ex: 24042022_1_3_2
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Tên khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Tổng thông số mặc định
        /// </summary>
        public int TotalParametersDefault { get; set; }

        /// <summary>
        /// Tổng thông số đã ghi
        /// </summary>
        public int TotalParametersChecked { get; set; }

        /// <summary>
        /// Tổng thông số đã được đồng bộ
        /// </summary>
        public int TotalParametersSynchronized { get; set; }

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
    }
}
