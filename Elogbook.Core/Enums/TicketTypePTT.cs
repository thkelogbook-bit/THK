using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại phiếu
    /// </summary>
    public enum TicketTypePTT
    {
        /// <summary>
        /// Kế hoạch
        /// </summary>
        [Description("Kế hoạch")]
        KeHoach = 1,
        /// <summary>
        /// Đột xuất
        /// </summary>
        [Description("Đột xuất")]
        DotXuat,
    }
}
