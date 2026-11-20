using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Không thực hiện bảo trì tự quản
    /// </summary>
    [Table("AmStopMaintenance")]
    public partial class AmStopMaintenance : EntityBase
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
        /// Lý do dừng bảo trì
        /// </summary>
        public int? AmReasonForStopID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmReasonForStopID))]
        public virtual AmReasonForStop AmReasonForStop { get; set; }

        /// <summary>
        /// Người dừng thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ImplementerID))]
        public virtual User Implementer { get; set; }

        /// <summary>
        /// Chữ ký Người dừng thực hiện
        /// </summary>
        public string ImplementerSignatureContent { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian dừng
        /// </summary>
        public DateTime StopTime { get; set; } = DateTime.Now;

        /// <summary>
        /// SyncDataKey
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

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
