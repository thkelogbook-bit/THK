using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.OperationShift
{
    /// <summary>
    /// Định nghĩa các điều kiện tìm kiếm
    /// </summary>
    public class SearchOperationShiftModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Kiểu ngày cần tìm kiếm, 1: Ngày tạo, 2: Ngày sử dụng
        /// </summary>
        public int DateType { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
