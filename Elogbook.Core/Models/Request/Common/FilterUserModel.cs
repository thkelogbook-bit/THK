using Elogbook.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Autocomplete
    /// </summary>
    public class FilterUserModel
    {
        /// <summary>
        /// Nội dung tìm kiếm
        /// </summary>
        public string SearchKey { get; set; } = string.Empty;

        /// <summary>
        /// Mã số công ty
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// danh sách quyền
        /// </summary>
        public List<string> PermissionKey { get; set; }

        /// <summary>
        /// Loại người dùng (-1: tất cả, 1: trong hệ thống, 2: ngoài hệ thống)
        /// </summary>
        public int UserType { get; set; } = CommonConstant.ALL_KEY;
    }
}
