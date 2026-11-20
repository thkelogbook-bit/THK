using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại job history
    /// </summary>
    public enum JobHistoryType
    {
        /// <summary>
        /// Đồng bộ từ job tự động
        /// </summary>
        [Description("Auto")]
        Auto = 1,
        /// <summary>
        /// Đồng bộ từ màn hình
        /// </summary>
        [Description("Manual")]
        Manual = 2,
        
    }
}
