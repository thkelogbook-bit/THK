using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// loại đồng bộ
    /// </summary>
    public enum PmisSyncType
    {
        /// <summary>
        /// Thiết bị
        /// </summary>
        [Description("Đồng bộ thiết bị")]
        AmDevice = 1,
        /// <summary>
        /// Loại thiết bị
        /// </summary>
        [Description("Loại thiết bị")]
        AmDeviceType,
        /// <summary>
        /// Khu vực
        /// </summary>
        [Description("Khu vực")]
        AmLocation,
        /// <summary>
        /// Phân loại thiết bị
        /// </summary>
        [Description("Phân Loại thiết bị")]
        AmDeviceCategory,
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        [Description("Nhà cung cấp")]
        AmSupplier,
        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        [Description("Nhà chế tạo")]
        AmManufacturer,
        /// <summary>
        /// Nước sản xuất
        /// </summary>
        [Description("Nước sản xuất")]
        AmCountryManufacture
    }
}
