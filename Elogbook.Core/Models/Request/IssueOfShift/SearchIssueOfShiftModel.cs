using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Tồn tại trong ca (Web)
    /// </summary>
    public class SearchIssueOfShiftModel : BaseListFilter
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
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Tình trạng
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Từ khóa search
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Người gửi yêu cầu
        /// </summary>
        public int UserID { get; set; }
    }
}
