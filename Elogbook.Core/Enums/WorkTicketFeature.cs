using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại phiếu
    /// </summary>
    public enum WorkTicketFeature
    {
        /// <summary>
        /// Thủy
        /// </summary>
        [Description("Thủy")]
        Thuy = 1,
        /// <summary>
        /// Cơ
        /// </summary>
        [Description("Cơ")]
        Co = 2,
        /// <summary>
        /// Nhiệt
        /// </summary>
        [Description("Nhiệt")]
        Nhiet = 3,
        /// <summary>
        /// Hóa
        /// </summary>
        [Description("Hóa")]
        Hoa = 4,
        /// <summary>
        /// Xay Dung
        /// </summary>
        [Description("Xây dựng")]
        XayDung = 5,
    }
}
