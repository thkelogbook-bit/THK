using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa table AM
    /// </summary>
    public enum EnumAmCategories
    {
        /// <summary>
        /// Loại công việc
        /// </summary>
        [Description("Loại công việc")]
        WorkType = 1,

        /// <summary>
        /// Công việc
        /// </summary>
        [Description("Công việc")]
        Work,

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        [Description("Mức độ khẩn cấp")]
        Emergency,

        /// <summary>
        /// Khu vực
        /// </summary>
        [Description("Khu vực")]
        Location,

        /// <summary>
        /// Nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("Nguyên nhân tạm dừng công việc")]
        ReasonForPending,

        /// <summary>
        /// Nguyên nhân không thực hiện
        /// </summary>
        [Description("Nguyên nhân không thực hiện")]
        ReasonForStop,

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        [Description("Nhà cung cấp")]
        Supplier,

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        [Description("Nước sản xuất")]
        CountryManufacture,

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        [Description("Nhà chế tạo")]
        Manufacturer,

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        [Description("Loại thiết bị")]
        DeviceType,

        /// <summary>
        /// Phân loại thiết bị
        /// </summary>
        [Description("Phân loại thiết bị")]
        DeviceCategory,

        /// <summary>
        /// Thiết bị
        /// </summary>
        [Description("Thiết bị")]
        Device,
    }
}
