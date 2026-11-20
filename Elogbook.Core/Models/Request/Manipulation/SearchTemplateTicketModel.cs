using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// SearchTemplateTicketModel
    /// </summary>
    public class SearchTemplateTicketModel : BaseListFilter
    {
        /// <summary>
        /// Từ khóa
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = Constants.CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int TicketStatusValue { get; set; } = Constants.CommonConstant.ALL_KEY;

        /// <summary>
        /// Ngày tạo (từ ngày)
        /// </summary>
        public string FromDate { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo (đến ngày)
        /// </summary>
        public string ToDate { get; set; } = string.Empty;
    }
}
