using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu phiếu đăng ký công tác theo trạng trái
    /// </summary>
    public class SearchTicketByStatusModel : BaseListFilter
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }
    }
}
