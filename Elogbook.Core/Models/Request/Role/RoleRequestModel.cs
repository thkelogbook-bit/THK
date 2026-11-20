using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm vai trò
    /// </summary>
    public class RoleRequestModel : BaseListFilter
    {
        /// <summary>
        /// Tên vai trò
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Tất cả tương ứng với -1
        /// </summary>
        public int DataPermission { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tất cả tương ứng với -1
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
