using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình ngày làm việc trong tuần
    /// </summary>
    [Table("WorkScheduleDay")]
    public partial class WorkScheduleDay : EntityBase
    {
        /// <summary>
        /// ID của lịch làm việc
        /// </summary>
        public int? WorkScheduleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkScheduleID))]
        public virtual WorkSchedule WorkSchedule { get; set; }

        /// <summary>
        /// Ngày trong tuần (0: Chủ nhật, 1: Thứ hai, ..., 6: Thứ bảy)
        /// </summary>
        public int DayOfWeek { get; set; }

        /// <summary>
        /// Phân loại ngày làm việc (1: Giờ tiêu chuẩn, 2: Giờ tùy chỉnh, 3: Không thiết lập)
        /// </summary>
        public int WorkingType { get; set; }

        /// <summary>
        /// Có phải là ngày làm việc không (true: Là ngày làm việc, false: Không phải là ngày làm việc)
        /// </summary>
        public bool IsWorkingDay { get; set; } = true;

        /// <summary>
        /// Thời gian bắt đầu (HH:mm)
        /// </summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc (HH:mm)
        /// </summary>
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Thời gian làm việc (tính bằng phút)
        /// </summary>
        public int WorkDurationMinutes { get; set; }

        /// <summary>
        /// Danh sách thời gian nghỉ trong ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkScheduleDayBreak> WorkScheduleDayBreaks { get; set; }
    }
}
