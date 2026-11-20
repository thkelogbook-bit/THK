using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình lịch làm việc
    /// </summary>
    [Table("WorkSchedule")]
    public partial class WorkSchedule : EntityBase
    {
        /// <summary>
        /// Tên lịch làm việc
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Loại lịch làm việc (1: 24h, 2: Standard)
        /// </summary>
        public int WorkType { get; set; }

        /// <summary>
        /// Thời gian bắt đầu chuẩn (HH:mm)
        /// </summary>
        public TimeSpan? StandardStartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc chuẩn (HH:mm)
        /// </summary>
        public TimeSpan? StandardEndTime { get; set; }

        /// <summary>
        /// Thời gian làm việc chuẩn (tính bằng phút)
        /// </summary>
        public int StandardWorkDurationMinutes { get; set; }

        /// <summary>
        /// Trạng thái áp dụng (true: Áp dụng, false: Không áp dụng)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Thông tin thời gian nghỉ trong lịch làm việc
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkScheduleBreak> WorkScheduleBreaks { get; set; }

        /// <summary>
        /// Thông tin ngày làm việc trong lịch làm việc
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkScheduleDay> WorkScheduleDays { get; set; }
    }
}
