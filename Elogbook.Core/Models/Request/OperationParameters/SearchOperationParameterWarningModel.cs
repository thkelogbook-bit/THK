using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tìm kiếm danh sách thông số vượt ngưỡng cảnh báo
    /// </summary>
    public class SearchOperationParameterWarningModel : BaseListFilter
    {
        /// <summary>
        /// Mức độ cảnh báo
        /// </summary>
        public int WarningLevel { get; set; }
    }
}
