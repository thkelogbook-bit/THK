using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// tham số tìm kiếm địa điểm
    /// </summary>
    public class AmLocationSearchModel : BaseListFilter
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public int OrganizationID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Khu vực cha
        /// </summary>
        public int SiteParent { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên or mã khu vực
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
