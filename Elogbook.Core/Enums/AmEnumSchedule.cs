using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại lịch bảo trì
    /// </summary>
    public enum AmEnumSchedule
    {
        /// <summary>
        /// Hàng ngày
        /// </summary>
        [Description("Hàng ngày")]
        Daily = 1,
        /// <summary>
        /// Hàng tuần
        /// </summary>
        [Description("Hàng tuần")]
        Weekly,
        /// <summary>
        /// Hàng tháng
        /// </summary>
        [Description("Hàng tháng")]
        Monthly,
        /// <summary>
        /// Hàng năm
        /// </summary>
        [Description("Hàng năm")]
        Yearly,
    }
}
