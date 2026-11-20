using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số nhận từ UI
    /// </summary>
    public class WorkScheduleRequestModel
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
        /// Phân loại lịch làm việc (1: 24h, 2: Giờ làm việc  chuẩn)
        /// </summary>
        public int WorkType { get; set; }

        /// <summary>
        /// Thời gian chuẩn bắt đầu (HH:mm)
        /// </summary>
        public string StandardStartTime { get; set; }

        /// <summary>
        /// Thời gian chuẩn kết thúc (HH:mm)
        /// </summary>
        public string StandardEndTime { get; set; }

        /// <summary>
        /// Thời gian làm việc chuẩn (tính bằng phút)
        /// </summary>
        public int StandardWorkDurationMinutes { get; set; }

        /// <summary>
        /// Trạng thái áp dụng (true: Áp dụng, false: Không áp dụng)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Danh sách thời gian nghỉ
        /// </summary>
        public List<WorkScheduleBreakRequestModel> BreakDays { get; set; }

        /// <summary>
        /// Danh sách ngày làm việc trong tuần
        /// </summary>
        public List<WorkScheduleDayRequestModel> WorkDays { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }

    /// <summary>
    /// Tham số nhận từ UI cho thời gian nghỉ
    /// </summary>
    public class WorkScheduleBreakRequestModel
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
        public string BreakStartTime { get; set; }
        /// <summary>
        /// Thời gian kết thúc nghỉ (HH:mm)
        /// </summary>
        public string BreakEndTime { get; set; }

        /// <summary>
        /// Thời gian nghỉ (tính bằng phút)
        /// </summary>
        public int BreakDurationMinutes { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Tham số nhận từ UI cho thời gian làm việc trong tuần
    /// </summary>
    public class WorkScheduleDayRequestModel
    {
        /// <summary>
        /// ID của ngày làm việc
        /// </summary>
        public int ID { get; set; }

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
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc (HH:mm)
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Thời gian làm việc (tính bằng phút)
        /// </summary>
        public int WorkDurationMinutes { get; set; }

        /// <summary>
        /// Danh sách thời gian nghỉ trong ngày này
        /// </summary>
        public List<WorkScheduleDayBreakRequestModel> DayBreaks { get; set; }

        /// <summary>
        /// Xác định chế độ thêm mới hay cập nhật (true: Thêm mới, false: Cập nhật)
        /// </summary>
        public bool IsAddMode { get; set; } = true;

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Tham số nhận từ UI cho thời gian nghỉ trong ngày
    /// </summary>
    public class WorkScheduleDayBreakRequestModel
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
        public string BreakStartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc nghỉ (HH:mm)
        /// </summary>
        public string BreakEndTime { get; set; }

        /// <summary>
        /// Thời gian nghỉ (tính bằng phút)
        /// </summary>
        public int BreakDurationMinutes { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
