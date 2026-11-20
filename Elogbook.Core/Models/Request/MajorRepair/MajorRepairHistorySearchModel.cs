using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông số tìm kiếm
    /// </summary>
    public class MajorRepairHistorySearchModel : BaseListFilter
    {
        /// <summary>
        /// ID của SCL
        /// </summary>
        public int MajorRepairID { get; set; }

        /// <summary>
        /// Thời gian phát sinh (Từ)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Thời gian phát sinh (Đến)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Nội dung tìm kiếm
        /// </summary>
        public string SearchText { get; set; }
    }
}
