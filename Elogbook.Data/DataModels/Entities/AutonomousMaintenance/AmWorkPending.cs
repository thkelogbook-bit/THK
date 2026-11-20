using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Tạm dừng hoặc Dừng thực hiện công việc
    /// </summary>
    [Table("AmWorkPending")]
    public partial class AmWorkPending : EntityBase
    {
        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        [Required]
        public int AutonomousMaintenanceID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AutonomousMaintenanceID))]
        public virtual AutonomousMaintenance AutonomousMaintenance { get; set; }

        /// <summary>
        /// Mã công việc
        /// </summary>
        [Required]
        public int AmWorkDetailID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkDetailID))]
        public virtual AmWorkDetail AmWorkDetail { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        [Required]
        public int AmLocationID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmLocationID))]
        public virtual AmLocation AmLocation { get; set; }

        /// <summary>
        /// Lý do tạm dừng công việc
        /// </summary>
        public int? AmReasonForPendingID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmReasonForPendingID))]
        public virtual AmReasonForPending AmReasonForPending { get; set; }

        /// <summary>
        /// Lý do dừng công việc
        /// </summary>
        public int? AmReasonForStopID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmReasonForStopID))]
        public virtual AmReasonForStop AmReasonForStop { get; set; }

        /// <summary>
        /// Người tạm dừng thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ImplementerID))]
        public virtual User Implementer { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng
        /// </summary>
        public DateTime PendingTime { get; set; } = DateTime.Now;

        /// <summary>
        /// SyncDataKey
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

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
