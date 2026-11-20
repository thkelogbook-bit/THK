using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Chu kỳ
    /// </summary>
    public class SearchCycleManagementModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Tên chu kỳ
        /// </summary>
        public string CycleName { get; set; }
    }
}
