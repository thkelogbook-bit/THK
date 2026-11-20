using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu thao tác ở web
    /// </summary>
    public class SearchTicketWebModel : BaseListFilter
    {
        /// <summary>
        /// Từ khóa
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } =  CommonConstant.ALL_KEY;

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public int ApproveUnitID { get; set; } = CommonConstant.ALL_KEY;

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
        public int TicketType { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int TicketStatusID { get; set; } = CommonConstant.ALL_KEY;
    }
}
