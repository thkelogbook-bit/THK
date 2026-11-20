using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// tham số tìm kiếm
    /// </summary>
    public class AmReasonForPendingSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
