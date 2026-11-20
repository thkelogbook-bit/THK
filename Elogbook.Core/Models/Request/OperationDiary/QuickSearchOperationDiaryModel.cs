using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tìm kiếm nhanh thông tin NKVH
    /// </summary>
    public class QuickSearchOperationDiaryModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Năm vận hành
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Nội dung tìm kiếm
        /// </summary>
        public string SearchText { get; set; }
    }
}
