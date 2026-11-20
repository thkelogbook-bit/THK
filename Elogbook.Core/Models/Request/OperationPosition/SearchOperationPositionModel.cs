using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Cương vị
    /// </summary>
    public class SearchOperationPositionModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Kiểu xác thực Sử dụng 1/RFID - 2/QR Code
        /// </summary>
        public int AthenticationType { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }
    }
}
