using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số tìm kiếm 
    /// </summary>
    public class AmSupplierSearchModel : BaseListFilter
    {
        /// <summary>
        /// Tên or Mã
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
