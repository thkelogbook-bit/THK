using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu thao tác ở mobile
    /// </summary>
    public class SearchTicketMobileModel : BaseListFilter
    {
        /// <summary>
        /// Tab
        /// </summary>
        public int? Tab { get; set; }

        /// <summary>
        /// Từ khóa
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Từ khóa
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }
    }
}
