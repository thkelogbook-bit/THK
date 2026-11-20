using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Phiếu công tác Điện
    /// </summary>
    public enum PCTDFormat
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
        /// Tên viết tắt nhà máy
        /// </summary>
        [Description("Tên viết tắt nhà máy")]
        FactoryShortName = 3,
        /// <summary>
        /// Tên viết tắt phân xưởng đơn vị cấp phiếu
        /// </summary>
        [Description("Tên viết tắt phân xưởng đơn vị cấp phiếu")]
        WorkshopUnitShortName = 4,
        /// <summary>
        /// Tên viết tắt công ty
        /// </summary>
        [Description("Tên viết tắt công ty")]
        CompanyShortName = 5,
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
