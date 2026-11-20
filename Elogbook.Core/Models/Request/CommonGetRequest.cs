using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// CommonGetRequest
    /// </summary>
    public class CommonGetRequest: BaseListFilter
    {
        /// <summary>
        /// PageSize
        /// </summary>
        public new int PageSize { get; set; } = int.MaxValue;
    }
}
