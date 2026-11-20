using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm tài liệu
    /// </summary>
    public class KnowledgeBaseRequestModel : BaseListFilter
    {
        /// <summary>
        /// Chủ đề tài liệu
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Tất cả tương ứng với -1
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Lọc theo giá trị module (null = tất cả)
        /// </summary>
        public string ModuleValue { get; set; }
    }
}
