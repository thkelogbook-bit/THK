using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Thông tin tìm kiếm phiếu đăng ký thao tác
    /// </summary>
    public class SearchRegistrationManipulationTicketModel : BaseListFilter
    {
        /// <summary>
        /// Từ khóa
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên đơn vị cấp phiếu
        /// </summary>
        public int ApproveUnitID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Ngày tạo phiếu (từ ngày)
        /// </summary>
        public string FromDate { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo phiếu (đến ngày)
        /// </summary>
        public string ToDate { get; set; } = string.Empty;

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int TicketType { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int TicketStatusValue { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// True = tìm kiếm từ màn hình Tạo phiếu thao tác, False = tìm kiếm từ màn hình Cấp phiếu thao tác
        /// </summary>
        public bool IsRegistrationTicket { get; set; }
    }
}
