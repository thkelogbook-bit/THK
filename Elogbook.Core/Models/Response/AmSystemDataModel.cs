using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin dữ liệu dùng chung của AM
    /// </summary>
    public class AmSystemDataModel
    {
        /// <summary>
        /// Tình trạng sử dụng
        /// </summary>
        public List<ValueLabelCheckedModel> AmUsageStatus { get; set; }

        /// <summary>
        /// Trạng thái thực hiện
        /// </summary>
        public List<ValueLabelCheckedModel> AmStatus { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public List<ComboboxDataModel> AmDevices { get; set; }

        /// <summary>
        /// Loại Thiết bị
        /// </summary>
        public List<ComboboxDataModel> AmDeviceTypes { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public List<ComboboxDataModel> AmLocations { get; set; }

        /// <summary>
        /// Nguyên nhân tạm dừng
        /// </summary>
        public List<ValueLabelCheckedModel> AmReasonForPendings { get; set; }

        /// <summary>
        /// Nguyên nhân dừng thực hiện
        /// </summary>
        public List<ComboboxDataModel> AmReasonForStops { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public List<ValueLabelCheckedModel> AmWorks { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public List<ValueLabelCheckedModel> AmWorkTypes { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public List<ValueLabelCheckedModel> AmDeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Tình trạng thiết bị
        /// </summary>
        public List<ValueLabelCheckedModel> AmDeviceStatus { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public List<ComboboxDataModel> AmManufacturers { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public List<ComboboxDataModel> AmSuppliers { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public List<ComboboxDataModel> AmCountryManufactures { get; set; }

        /// <summary>
        /// Phân loại thiết bị
        /// </summary>
        public List<ComboboxDataModel> AmDeviceCategories { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public List<ValueLabelCheckedModel> AmEmergencies { get; set; }

        /// <summary>
        /// Phân loại nguyên nhân
        /// </summary>
        public List<ValueLabelCheckedModel> AmReasonType { get; set; }
    }
}
