using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Dữ liệu danh mục
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// Danh sách nhân viên trong nhà máy
        /// </summary>
        public List<UserInfo> Users { get; set; }
    }
}
