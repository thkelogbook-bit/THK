using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Các tham số tìm kiếm
    /// </summary>
    public class SearchTicketEventModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Loại phiếu CTĐ, TCNH
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Event Type
        /// </summary>
        public int EventTypeID { get; set; }

        /// <summary>
        /// Nội dung mẫu
        /// </summary>
        public string EventContent { get; set; }
    }
}
