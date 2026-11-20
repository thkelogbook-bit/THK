using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// khai báo model tìm kiếm thời gian xử lý bất thường
    /// </summary>
    public class SearchTimeHandleModel : BaseListFilter
    {
        /// <summary>
        /// nhà máy
        /// </summary>
        public int FactoryId { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// tháng
        /// </summary>
        public int Month { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// mức độ khẩn cấp
        /// </summary>
        public int Emergency { get; set; } = CommonConstant.ALL_KEY;
    }
}
