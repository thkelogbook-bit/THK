using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin tiếp nhận nơi làm việc
    /// </summary>
    public class LctWorkProgressModel : EntityBaseModel
    {
        /// <summary>
        /// lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Đã kiểm tra những biện pháp an toàn tại hiện trường
        /// </summary>
        public string CheckedSafetyMeasuresAtScene { get; set; }

        /// <summary>
        /// Đã làm thêm các biện pháp an toàn và tiếp đất tại
        /// </summary>
        public string AddedSafetyMeasuresAndGrounding { get; set; }

        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        public DateTime? StartWorkingDate { get; set; }
    }
}
