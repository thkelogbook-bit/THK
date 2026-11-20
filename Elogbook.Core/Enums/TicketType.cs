using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại phiếu công tác
    /// </summary>
    public enum TicketType
    {
        /// <summary>
        /// Kế hoạch
        /// </summary>
        [Description("Kế hoạch")]
        InsidePlan = 1,
        /// <summary>
        /// Ngoài kế hoạch
        /// </summary>
        [Description("Ngoài kế hoạch")]
        OutsidePlan,
        /// <summary>
        /// Xử lý sự cố
        /// </summary>
        [Description("Xử lý sự cố")]
        SpecialCase,
        /// <summary>
        /// Phiếu công tác thuộc Đợt công tác SCL
        /// </summary>
        [Description("Đợt công tác SCL")]
        MajorRepairCase,
    }
}
