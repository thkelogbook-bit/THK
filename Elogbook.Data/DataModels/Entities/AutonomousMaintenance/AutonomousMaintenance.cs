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
    /// Bảo trì tự quản - AutonomousMaintenance
    /// </summary>
    [Table("AutonomousMaintenance")]
    public partial class AutonomousMaintenance : EntityBase
    {
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
        /// Lịch thực hiện
        /// </summary>
        [Required]
        public int AmMaintenanceScheduleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmMaintenanceScheduleID))]
        public virtual AmMaintenanceSchedule AmMaintenanceSchedule { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [Required]
        public int AmStatusID { get; set; } = (int)AmEnumStatus.Waiting;

        [MapperIgnore]
        [ForeignKey(nameof(AmStatusID))]
        public virtual AmStatus AmStatus { get; set; }

        /// <summary>
        /// Có bất thường
        /// </summary>
        public bool? IsAbnormal { get; set; } = false;

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public int? AmDeviceAbnormalStatusID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceAbnormalStatusID))]
        public virtual AmDeviceAbnormalStatus AmDeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Danh sách công việc AM được thực hiện
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<AmWorkDetail> AmWorkDetails { get; set; }

        /// <summary>
        /// File hình ảnh trước khi thực hiện AM
        /// </summary>
        public string BeforeImages { get; set; }

        /// <summary>
        /// File hình ảnh sau khi thực hiện AM
        /// </summary>
        public string AfterImages { get; set; }

        /// <summary>
        /// Mô tả bất thường thiết bị
        /// </summary>
        public string DeviceAbnormalDescription { get; set; }

        /// <summary>
        /// Các file bất thường thiết bị, Hình ảnh or Video
        /// </summary>
        public string DeviceAbnormalAttachments { get; set; }

        /// <summary>
        /// ID phiếu yêu cầu
        /// </summary>
        public int? AmRequestTicketID { get; set; }

        /// <summary>
        /// Người hoành thành
        /// </summary>
        public int? CompleterUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CompleterUserID))]
        public virtual User CompleterUser { get; set; }

        /// <summary>
        /// Chữ ký Người hoành thành
        /// </summary>
        public string CompleterSignatureContent { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? CompleteDate { get; set; }

        /// <summary>
        /// Đã gửi mail nhắc lịch 
        /// </summary>
        public bool IsSendNotify { get; set; } = false;

        /// <summary>
        /// Tự động hoàn thành
        /// </summary>
        public bool IsAutoComplete { get; set; } = false;

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

        /// <summary>
        /// Đánh dấu việc đồng bộ dữ liệu AM sang NKVH
        /// </summary>
        public bool IsSyncToOperationDiary { get; set; }
    }
}
