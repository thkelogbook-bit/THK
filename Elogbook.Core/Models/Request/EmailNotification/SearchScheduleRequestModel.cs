using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm schedule
    /// </summary>
    public class SearchScheduleRequestModel : BaseListFilter
    {
        /// <summary>
        /// Tên công việc, Nội dung công việc
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// ID cấu hình SLA
        /// </summary>
        public int? SlaConfigID { get; set; } = null;

        /// <summary>
        /// ID phân loại SLA liên quan
        /// </summary>
        public int? SlaCategoryID { get; set; } = null;
    }
}
