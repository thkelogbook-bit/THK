using System;
using System.Collections.Generic;

namespace Elogbook.Core
{
    /// <summary>
    /// Lấy các mốc thời gian gửi thông báo
    /// </summary>
    public static class WorkPermitConfig
    {
        /// <summary>
        /// Lấy giá trị một lần khi ứng dụng chạy hoặc khởi động lại
        /// </summary>
        public static readonly NotificationTimeConfig CompletionNotificationTimes = Load();

        /// <summary>
        /// Giá trị round time để lấy thông tin phiếu
        /// </summary>
        public static readonly int NotificationWindowMinutes = AppSettings.WORK_PERMIT_NOTIFICATION_WINDOW_MINUTES;

        private static NotificationTimeConfig Load()
        {
            var beforeTimes = LoadTimeList(AppSettings.WORK_TICKET_BEFORE_COMPLETION_NOTIFICATION_HOURS, new[] { 12 });

            var afterTimes = LoadTimeList(AppSettings.WORK_TICKET_AFTER_COMPLETION_NOTIFICATION_HOURS, new[] { 24, 48 });

            return new NotificationTimeConfig(beforeTimes, afterTimes);
        }

        private static List<TimeSpan> LoadTimeList(string value, int[] defaults)
        {
            var result = new List<TimeSpan>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                foreach (var item in value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(item.Trim(), out int hours) && hours > 0)
                    {
                        result.Add(TimeSpan.FromHours(hours));
                    }
                }
            }

            if (result.Count == 0)
            {
                foreach (var hour in defaults)
                {
                    result.Add(TimeSpan.FromHours(hour));
                }
            }

            return result;
        }

    }
}