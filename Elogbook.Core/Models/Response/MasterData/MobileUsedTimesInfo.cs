using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các khoản thời gian sử dụng ca vận hành
    /// </summary>
    public class MobileUsedTimesInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Thời gian bắt đầu sử dụng ca vận hành
        /// </summary>
        public DateTime? ActiveTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc sử dụng ca vận hành
        /// </summary>
        public DateTime? InactiveTime { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int? OperationShiftID { get; set; }
    }
}
