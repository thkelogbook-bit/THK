using Elogbook.Core.Constants;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// QuickAutoCompleteModel
    /// </summary>
    public class QuickAutoCompleteModel
    {
        /// <summary>
        /// Key search
        /// </summary>
        public string KeySearch { get; set; }

        /// <summary>
        /// Tên table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// PageNumber
        /// </summary>
        public int PageNumber { get; set; } = CommonConstant.DEFAULT_PAGE_NUMBER;

        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; } = int.MaxValue;

    }
}
