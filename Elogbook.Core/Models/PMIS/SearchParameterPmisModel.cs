using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.PMIS
{
    /// <summary>
    /// Thông tin tìm kiếm thông số PMIS
    /// </summary>
    public class SearchParameterPmisModel : BaseListFilter
    {
        /// <summary>
        /// Mã nhóm thông số
        /// </summary>
        public string OPTABID { get; set; }
    }
}
