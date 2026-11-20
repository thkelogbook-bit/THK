using Elogbook.Core.Constants;

namespace Elogbook.Core.Models.BaseFilter
{
    /// <summary>
    /// Những thông tin chung cho hàm tìm kiếm dữ liệu
    /// </summary>
    public class BaseListFilter
    {
        private const string DEFAULT_SORT = "ASC";
        private int _pageSize = CommonConstant.DEFAULT_PAGE_SIZE;

        /// <summary>
        /// Cột trên lưới cần sắp xếp
        /// </summary>
        public string SortColumn { get; set; } = string.Empty;

        /// <summary>
        /// Sắp xếp dữ liệu theo chiều tăng dần/giảm dần
        /// </summary>
        public string SortOrder { get; set; } = DEFAULT_SORT;

        /// <summary>
        /// Số dòng dữ liệu trên mỗi page
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value == 0 ? int.MaxValue : value;
            }
        }

        /// <summary>
        /// Page cần hiển thị dữ liệu
        /// </summary>
        public int PageNumber { get; set; } = CommonConstant.DEFAULT_PAGE_NUMBER;
    }
}
