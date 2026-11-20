namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationTimeLine")]
    public partial class OperationTimeLine : EntityBase
    {
        [StringLength(100)]
        public string TimeLineSyncDataKey { get; set; }
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Tên khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

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
