using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Tổ máy
    /// </summary>
    public class SearchPowerUnitModel : BaseListFilter
    {
        /// <summary>
        /// Tên tổ máy
        /// </summary>
        public string PowerUnitName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
