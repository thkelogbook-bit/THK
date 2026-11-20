namespace Elogbook.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Bảng lưu trữ thông số vận hành đã được archive từ bảng OperationParameters.
    /// Dữ liệu cũ hơn 12 tháng sẽ được chuyển sang bảng này thông qua HistoryCallJob.
    ///
    /// Composite PK (InsertedAt, ID) được cấu hình qua ElogbookDataContext.OnModelCreating.
    /// InsertedAt là partition key — đặt trước trong clustered index.
    /// Không có FK navigation properties vì bảng archive không có FK constraints.
    /// </summary>
    [Table("OperationParametersArchive")]
    public partial class OperationParametersArchive
    {
        /// <summary>
        /// Ngày tạo — partition key, giữ nguyên giá trị từ bảng gốc
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// ID gốc từ bảng OperationParameters — không phải IDENTITY
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        /// <summary>
        /// Thời điểm bản ghi được archive (job chạy lúc nào)
        /// </summary>
        public DateTime ArchivedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Tên thông số (snapshot tại thời điểm ghi)
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = true)]
        public string ParameterName { get; set; }

        /// <summary>
        /// Thông số ID (tham chiếu danh mục Parameters)
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
        [StringLength(255)]
        public string KKSCode { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        [StringLength(255)]
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
        /// Kiểu dữ liệu (true = số, false = text)
        /// </summary>
        public bool IsNumberType { get; set; }

        /// <summary>
        /// Giá trị trước đó
        /// </summary>
        public string PreviousValue { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Đã được ghi hay chưa
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Ngày cập nhật dữ liệu
        /// </summary>
        public DateTime? CheckedDate { get; set; }

        /// <summary>
        /// Danh sách cảnh báo (denormalized từ AlertDetails)
        /// </summary>
        public string AlertDetails { get; set; }

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Người cập nhật cuối
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Key đồng bộ mobile
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; }

        /// <summary>
        /// App ID (mobile sync)
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime? SyncedDate { get; set; }

        /// <summary>
        /// Index cột Excel AI mapping
        /// </summary>
        public int? ExcelAIColIndex { get; set; }

        /// <summary>
        /// Snapshot giờ ghi từ OperationTimeLine (6, 12, 18, 0).
        /// Lưu lại để query không cần join OperationTimeLine sau khi bảng đó bị archive.
        /// </summary>
        public int? TimeLineName { get; set; }
    }
}
