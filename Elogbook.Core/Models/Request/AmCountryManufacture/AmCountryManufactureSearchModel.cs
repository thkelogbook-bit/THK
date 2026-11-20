using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số tìm kiếm 
    /// </summary>
    public class AmCountryManufactureSearchModel : BaseListFilter
    {
        /// <summary>
        /// Tên or Code
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
