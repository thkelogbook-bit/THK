using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại phiếu công tác
    /// </summary>
    public enum MajorRepairWorkTicketType
    {
        /// <summary>
        /// Chưa thực hiện cấp phiếu
        /// </summary>
        [Description("Phiếu công tác điện")]
        PCTD = 1,
        /// <summary>
        /// Đang cấp phiếu hàng loạt
        /// </summary>
        [Description("Phiếu công tác thủy cơ nhiệt hóa")]
        PCTTCNH = 2,        
    }
}
