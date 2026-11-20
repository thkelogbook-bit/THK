using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Tiến trình thực hiện AM
    /// </summary>
    [Table("AmWorkingProgress")]
    public partial class AmWorkingProgress : EntityBase
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
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
        /// Mã chi tiết thực hiện công việc AM
        /// </summary>
        public int? AmWorkDetailID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkDetailID))]
        public virtual AmWorkDetail AmWorkDetail { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? AmWorkTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkTypeID))]
        public virtual AmWorkType AmWorkType { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public int? AmWorkID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkID))]
        public virtual AmWork AmWork { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ImplementerID))]
        public virtual User Implementer { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public int? ReasonID { get; set; }

        /// <summary>
        /// ReasonType : 1:Pending, 2:Stop
        /// </summary>
        public int? ReasonType { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Remark { get; set; }

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
