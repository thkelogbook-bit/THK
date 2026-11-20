using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu công tác
    /// </summary>
    public class SearchWorkTicketMobileModel : BaseListFilter
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Từ khoá
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// Ngày cấp (Từ ngày)
        /// </summary>
        public string RegisteredFromDate { get; set; }

        /// <summary>
        /// Ngày cấp (Đến ngày)
        /// </summary>
        public string RegisteredToDate { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tab ở mobile
        /// </summary>
        public int Tab { get; set; }

        /// <summary>
        /// Loại phiếu: -1/Tất cả 1/kế hoạch 2/Ngoài kế hoạch 3/Xử lý sự cố
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// true: PCT ở trạng thái chờ. false: các trạng thái còn lại
        /// </summary>
        public bool IsWaitingTicket { get; set; }

        /// <summary>
        /// True: Sort Mới -> Cũ //  False: Sort Cũ -> Mới
        /// </summary>
        public bool SortDescending { get; set; }

        /// <summary>
        /// WorkTicketFeature
        /// </summary>
        public int WorkTicketFeature { get; set; }
    }
}
