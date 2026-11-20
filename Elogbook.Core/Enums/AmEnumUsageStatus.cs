using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Tình trạng sử dụng
    /// </summary>
    public enum AmEnumUsageStatus
    {
        /// <summary>
        /// Áp dụng
        /// </summary>
        [Description("Áp dụng")]
        Apply = 1,

        /// <summary>
        /// Không Áp dụng
        /// </summary>
        [Description("Không áp dụng")]
        UnApply
    }
}
