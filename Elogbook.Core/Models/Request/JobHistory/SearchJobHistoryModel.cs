using Elogbook.Core.Models.BaseFilter;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm lịch sử đồng bộ
    /// </summary>
    public class SearchJobHistoryModel : BaseListFilter
    {
        /// <summary>
        /// Loại đồng bộ
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string SeachText { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }

  
    }
}
