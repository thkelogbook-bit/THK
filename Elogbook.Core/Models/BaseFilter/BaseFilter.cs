using Elogbook.Core.Models.BaseCommon;
using System.Collections.Generic;

namespace Elogbook.Core.Models.BaseFilter
{
    /// <summary>
    /// BaseFilter
    /// </summary>
    public class BaseFilter
    {
        /// <summary>
        /// Filters
        /// </summary>
        public IList<IFilter> Filters { get; set; }
    }
}
