using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái sử dụng dữ liệu
    /// </summary>
    public enum AccessStatus
    {
        /// <summary>
        /// Có thể sử dụng
        /// </summary>
        [Description("Có thể sử dụng")]
        Active = 1,
        /// <summary>
        /// Không thể sử dụng
        /// </summary>
        [Description("Không thể sử dụng")]
        InActive,
    }
}
