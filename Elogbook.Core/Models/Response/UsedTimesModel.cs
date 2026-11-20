using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thời gian sử dụng ca vận hành
    /// </summary>
    public class UsedTimesModel
    {
        /// <summary>
        /// Thời gian bắt đầu hoặt động của ca
        /// </summary>
        public DateTime? ActiveTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc hoạt động của ca
        /// </summary>
        public DateTime? InactiveTime { get; set; }

    }
}
