using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// model tìm kiếm
    /// </summary>
    public class SearchAmDeviceCategoryModel : BaseListFilter
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public int OrgID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// mã số phân loại thiết bị cha
        /// </summary>
        public int ParentId { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// mã/tên
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2 Không áp dụng
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
