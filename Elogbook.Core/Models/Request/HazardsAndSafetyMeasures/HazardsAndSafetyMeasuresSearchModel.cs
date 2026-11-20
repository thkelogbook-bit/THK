using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Điều kiện tìm kiếm
    /// </summary>
    public class HazardsAndSafetyMeasuresSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// Loại 1: Mối nguy, 2: Biện pháp an toàn
        /// </summary>
        public int ContentType { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
