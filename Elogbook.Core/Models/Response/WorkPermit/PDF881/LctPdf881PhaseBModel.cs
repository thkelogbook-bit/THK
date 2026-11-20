using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin thực hiện công tác
    /// </summary>
    public class LctPdf881PhaseBModel
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
        public List<LctSafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Tiêu đề thực hiện công việc
        /// </summary>
        public string WorkDailyTitle { get; set; }

        /// <summary>
        /// Danh sách Người tham gia thực hiện công việc
        /// </summary>
        public List<LctWorkDailyDetailModel> WorkDailys { get; set; }

        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Ngày ký
        /// </summary>
        public string CommanderSignTime { get; set; }
    }
}
