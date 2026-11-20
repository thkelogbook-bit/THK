using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số tìm kiếm WorkTicketTracking
    /// </summary>
    public class SearchWorkTicketTrackingModel : BaseListFilter
    {
        /// <summary>
        /// Tên phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Mã số của Người yêu cầu thay đổi
        /// </summary>
        public int RequesterUserID { get; set; }

        /// <summary>
        /// Mã số của Người cho phép hiện tại
        /// </summary>
        public int CurrentAllowerUserID { get; set; }

        /// <summary>
        /// Mã số của Người cho phép cần thay đổi
        /// </summary>
        public int NewAllowerUserID { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public bool StatusID { get; set; }
    }
}
