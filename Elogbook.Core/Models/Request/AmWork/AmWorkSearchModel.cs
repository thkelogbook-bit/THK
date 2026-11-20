using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm
    /// </summary>
    public class AmWorkSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int WorkTypeID { get; set; } = CommonConstant.ALL_KEY;
    }
}
