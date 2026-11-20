using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm nhóm người dùng
    /// </summary>
    public class SearchUserGroupModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên của Nhóm người dùng
        /// </summary>
        public string UserGroupName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
