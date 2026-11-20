using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại nguyên nhân
    /// </summary>
    public enum AmEnumReasonType
    {
        /// <summary>
        /// Không thực hiện AM
        /// </summary>
        [Description("Không thực hiện AM")]
        StopAM = 1,

        /// <summary>
        /// Dừng công việc
        /// </summary>
        [Description("Dừng công việc")]
        StopWork
    }
}
