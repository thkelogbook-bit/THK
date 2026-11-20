using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin thực hiện công tác
    /// </summary>
    public class Pdf881PhaseCModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Thực hiện công tác
        /// </summary>
        public string WorkUnit { get; set; }

        /// <summary>
        /// Số lượng người
        /// </summary>
        public string PersonNumber { get; set; }

        /// <summary>
        /// Các biện pháp an toàn
        /// </summary>
        public List<SafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Tiêu đề thực hiện công việc
        /// </summary>
        public string WorkDailyTitle { get; set; }

        /// <summary>
        /// Danh sách Người tham gia thực hiện công việc
        /// </summary>
        public List<WorkDailyDetailModel> WorkDailys { get; set; }

        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public string WorkResult { get; set; }
    }
}
