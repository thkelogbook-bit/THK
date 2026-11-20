using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số tìm kiếm lệnh đăng ký công tác theo trạng trái
    /// </summary>
    public class LctSearchTicketByStatusModel : BaseListFilter
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }
    }
}
