using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// các tham số tìm kiếm
    /// </summary>
    public class AmWorkTypeSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên loại công việc
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
