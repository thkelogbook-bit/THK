using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thời gian nghỉ
    /// </summary>
    [Table("WorkScheduleBreak")]
    public partial class WorkScheduleBreak : EntityBase
    {
        /// <summary>
        /// ID của lịch làm việc
        /// </summary>
        public int? WorkScheduleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkScheduleID))]
        public virtual WorkSchedule WorkSchedule { get; set; }

        /// <summary>
        /// Tên thời gian nghỉ
        /// </summary>
        public string BreakName { get; set; }

        /// <summary>
        /// Thời gian bắt đầu nghỉ (HH:mm)
        /// </summary>
        public TimeSpan? BreakStartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc nghỉ (HH:mm)
        /// </summary>
        public TimeSpan? BreakEndTime { get; set; }

        /// <summary>
        /// Thời gian nghỉ (tính bằng phút)
        /// </summary>
        public int BreakDurationMinutes { get; set; }
    }
}
