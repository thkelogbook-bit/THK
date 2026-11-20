using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Quản lý Công việc AM của một lịch trình
    /// </summary>
    [Table("AmMaintenanceWork")]
    public partial class AmMaintenanceWork : EntityBase
    {
        /// <summary>
        /// Mã lịch bảo trì
        /// </summary>
        public int AmMaintenanceScheduleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmMaintenanceScheduleID))]
        public virtual AmMaintenanceSchedule AmMaintenanceSchedule { get; set; }

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
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

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
