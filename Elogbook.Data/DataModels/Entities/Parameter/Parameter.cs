namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Parameters")]
    public partial class Parameter : EntityBase
    {
        /// <summary>
        /// Thuộc nhóm thông số hoặc thông số lẻ
        /// </summary>
        public bool IsGroup { get; set; }
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CycleID))]
        public virtual CycleManagement Cycle { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ParameterGroupID))]
        public virtual ParameterGroup ParameterGroup { get; set; }

        /// <summary>
        /// Mã RFID
        /// </summary>
        [StringLength(255)]
        public string RFIDCode { get; set; }

        /// <summary>
        /// Mã QR Code
        /// </summary>
        [StringLength(255)]
        public string QRCode { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        [StringLength(255)]
        [Required]
        public string ParameterName { get; set; }

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
        [StringLength(255)]
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
        /// Mã QLKT
        /// </summary>
        [StringLength(255)]
        public string QLKTCode { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Thông tin danh sách cảnh báo
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<AlertDetail> AlertDetails { get; set; }

        /// <summary>
        /// Tên column trên file ai excel, dùng để mapping Parameter với column name trên file excel
        /// </summary>
        public int? ExcelAIColIndex { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
