using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm người ngoài hệ thống
    /// </summary>
    public class SearchGuestUserModel : BaseListFilter
    {
        /// <summary>
        /// Nhập Họ và tên, số CMND/CCCD, số thẻ ATĐ hoặc số thẻ ATLĐ
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }
    }
}
