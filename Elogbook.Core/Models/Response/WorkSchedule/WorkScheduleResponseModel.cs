using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số trả lên UI
    /// </summary>
    public class WorkScheduleResponseModel
    {
        /// <summary>
        /// ID của lịch làm việc
        /// </summary>
        public int ID { get; set; }

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
        /// Danh sách ngày làm việc trong tuần
        /// </summary>
        public List<WorkScheduleBreakResponseModel> BreakDays { get; set; }

        /// <summary>
        /// Danh sách ngày làm việc trong tuần
        /// </summary>
        public List<WorkScheduleDayResponseModel> WorkDays { get; set; }

    }

    /// <summary>
    /// Tham số trả lên UI cho thời gian nghỉ
    /// </summary>
    public class WorkScheduleBreakResponseModel
    {
        /// <summary>
        /// ID của thời gian nghỉ
        /// </summary>
        public int ID { get; set; }

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

    /// <summary>
    /// Tham số trả lên UI cho ngày làm việc
    /// </summary>
    public class WorkScheduleDayResponseModel
    {
        /// <summary>
        /// ID của ngày làm việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày trong tuần (0: Chủ nhật, 1: Thứ 2, ..., 6: Thứ 7)
        /// </summary>
        public int DayOfWeek { get; set; }

        /// <summary>
        /// Phân loại ngày làm việc (1: Giờ tiêu chuẩn, 2: Giờ tùy chỉnh, 3: Không thiết lập)
        /// </summary>
        public int WorkingType { get; set; }

        /// <summary>
        /// Tên ngày trong tuần
        /// </summary>
        public string DayName { get; set; }

        /// <summary>
        /// Trạng thái làm việc (true: Làm việc, false: Nghỉ)
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
        /// Danh sách thời gian nghỉ trong ngày này
        /// </summary>
        public List<WorkScheduleDayBreakResponseModel> DayBreaks { get; set; }
    }

    /// <summary>
    /// Tham số trả lên UI cho thời gian nghỉ trong ngày
    /// </summary>
    public class WorkScheduleDayBreakResponseModel
    {
        /// <summary>
        /// ID của thời gian nghỉ trong ngày
        /// </summary>
        public int ID { get; set; }

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
