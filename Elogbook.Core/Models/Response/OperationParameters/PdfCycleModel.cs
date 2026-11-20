using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông số vận hành của 1 chu kỳ
    /// </summary>
    public class PdfCycleModel
    {
        /// <summary>
        /// Danh sách thông số vận hành của 1 chu kỳ
        /// </summary>
        public List<OperationParametersWebModel> ListOperationParameters { get; set; }
    }
}
