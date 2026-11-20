using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu công tác
    /// </summary>
    public class SearchWorkTicketModel : BaseListFilter
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int ApprovedUnitID { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// Trạng thái thay đổi người cho phép
        /// </summary>
        public int TrackingStatusID { get; set; }
    }
}
