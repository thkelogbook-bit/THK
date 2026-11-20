using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Báo cáo Tiến trình xử lý yêu cầu - Tổng hợp
    /// </summary>
    public class ReportPrtGeneralModel
    {
        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<PrtStatisticsReport> DataResult { get; set; }
    }
}
