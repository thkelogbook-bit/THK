using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại lệnh công tác
    /// </summary>
    public enum WorkPermitType
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
    }
}
