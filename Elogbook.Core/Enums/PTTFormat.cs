using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại phiếu thao tác
    /// </summary>
    public enum PTTFormat
    {
        /// <summary>
        /// Số phiếu
        /// </summary>
        [Description("Số phiếu")]
        Number = 1,
        /// <summary>
        /// Năm
        /// </summary>
        [Description("Năm")]
        Year = 2,
        /// <summary>
        /// Loại phiếu, KH: kế hoạch or ĐX: đột xuất
        /// </summary>
        [Description("Loại phiếu, KH: kế hoạch or ĐX: đột xuất")]
        Type = 3,
        /// <summary>
        /// Tên viết tắt nhà máy
        /// </summary>
        [Description("Tên viết tắt nhà máy")]
        FactoryShortName = 4,
        /// <summary>
        /// Tên viết tắt phân xưởng đơn vị cấp phiếu
        /// </summary>
        [Description("Tên viết tắt phân xưởng đơn vị cấp phiếu")]
        WorkshopUnitShortName = 5,
        /// <summary>
        /// dấu /
        /// </summary>
        [Description("/")]
        Slash = 6,
        /// <summary>
        /// dấu /
        /// </summary>
        [Description("-")]
        Hyphen = 7,
    }
}
