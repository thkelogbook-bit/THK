using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin file PDF
    /// </summary>
    public class PdfOperationParameters
    {
        /// <summary>
        /// Thông tin tiêu đề của file PDF
        /// </summary>
        public PdfHeaderOperationDiaryModel Header { get; set; }

        /// <summary>
        /// Danh sách thông số vận hành của tất cả chu kỳ
        /// </summary>
        public List<PdfCycleModel> ListCycle { get; set; }

        /// <summary>
        /// Danh sách thông tin nhật ký vận hành
        /// </summary>
        public List<PdfOperationDiaryInfoModel> ListOperationDiaryInfo { get; set; }
    }
}
