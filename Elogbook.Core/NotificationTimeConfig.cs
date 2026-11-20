using System;
using System.Collections.Generic;

namespace Elogbook.Core
{
    /// <summary>
    /// Các mốc thời gian lấy thông tin gửi thông báo
    /// </summary>
    public sealed class NotificationTimeConfig
    {
        /// <summary>
        /// Các mốc trước khi hết hạn
        /// Order giảm dần
        /// Ví dụ: 24h, 12h
        /// </summary>
        public IReadOnlyList<TimeSpan> BeforeTimes { get; private set; }

        /// <summary>
        /// Các mốc sau khi hết hạn
        /// Order tăng dần
        /// Ví dụ: 24h, 48h
        /// </summary>
        public IReadOnlyList<TimeSpan> AfterTimes { get; private set; }

        /// <summary>
        /// Lấy mốc thời gian gửi thông báo
        /// </summary>
        /// <param name="beforeTimes"></param>
        /// <param name="afterTimes"></param>
        public NotificationTimeConfig(
            IReadOnlyList<TimeSpan> beforeTimes,
            IReadOnlyList<TimeSpan> afterTimes)
        {
            BeforeTimes = beforeTimes;
            AfterTimes = afterTimes;
        }

    }
}