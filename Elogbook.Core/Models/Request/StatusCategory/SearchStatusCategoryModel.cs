using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm danh mục trạng thái
    /// </summary>
    public class SearchStatusCategoryModel : BaseListFilter
    {
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Loại dữ liệu trạng thái
        /// </summary>
        public int DataTypeID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
