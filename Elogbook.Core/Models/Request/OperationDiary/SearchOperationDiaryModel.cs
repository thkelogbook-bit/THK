using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Nhật ký vận hành
    /// </summary>
    public class SearchOperationDiaryModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Họ và tên người tạo
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int OperationCrewID { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Tìm kiếm dữ liệu theo điều kiện
        /// </summary>
        public bool IsFilter { get; set; }
    }
}
