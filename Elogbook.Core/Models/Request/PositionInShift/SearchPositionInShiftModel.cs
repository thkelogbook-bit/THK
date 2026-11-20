using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Cương vị trong ca
    /// </summary>
    public class SearchPositionInShiftModel : BaseListFilter
    {
        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }
    }
}
