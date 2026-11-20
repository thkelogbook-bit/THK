using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thông tin chi tiết thực hiện công việc AM
    /// </summary>
    [Table("AmWorkDetail")]
    public partial class AmWorkDetail : EntityBase
    {
        /// <summary>
        /// Mã bảo trì tự quản
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
        /// Loại công việc
        /// </summary>
        [Required]
        public int AmWorkTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkTypeID))]
        public virtual AmWorkType AmWorkType { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        [Required]
        public int AmWorkID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkID))]
        public virtual AmWork AmWork { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Required]
        public int AmStatusID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmStatusID))]
        public AmStatus AmStatus { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ImplementerID))]
        public virtual User Implementer { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// Mô tả hướng dẫn
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian lưu tạm (mm:ss)
        /// </summary>
        public string StickTime { get; set; }

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
