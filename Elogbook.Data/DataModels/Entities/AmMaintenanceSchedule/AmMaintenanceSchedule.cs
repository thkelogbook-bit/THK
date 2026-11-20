using Elogbook.Core.Enums;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Quản lý lịch thực hiện AM
    /// </summary>
    [Table("AmMaintenanceSchedule")]
    public partial class AmMaintenanceSchedule : EntityBase
    {
        /// <summary>
        /// Tên lịch trình
        /// </summary>
        [Required]
        [StringLength(255)]
        public string MaintenanceScheduleName { get; set; }

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
        /// Thiết bị
        /// </summary>
        [Required]
        public int AmDeviceID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceID))]
        public virtual AmDevice AmDevice { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        [Required]
        public int OperationPositionID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        [Required]
        public int OperationShiftID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(OperationShiftID))]
        public virtual OperationShift OperationShift { get; set; }

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2: Không Áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Danh sách công việc AM được thực hiện
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<AmMaintenanceWork> AmMaintenanceWorks { get; set; }

        /// <summary>
        /// Thời gian bắt đầu run cron job
        /// </summary>
        public string CronJobFormat { get; set; }

        /// <summary>
        /// Mô tả cron job
        /// </summary>
        public string CronJobDesc { get; set; }

        /// <summary>
        /// Thời gian nhắc lịch (số phút)
        /// </summary>
        public int NotificationsTime { get; set; } = -1;

        /// <summary>
        /// Ngày bắt đầu của 1 lịch trình
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue.Date;

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue.Date;

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
