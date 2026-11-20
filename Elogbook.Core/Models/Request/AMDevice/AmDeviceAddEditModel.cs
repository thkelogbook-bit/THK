using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số thêm mới/cập nhật của thiết bị
    /// </summary>
    public class AmDeviceAddEditModel
    {
        /// <summary>
        /// ID thiết bị
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public int? TypeDescID { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        /// Thiết bị cha
        /// </summary>
        public int? AssetParent { get; set; }

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string AssetDesc { get; set; }

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// Tình trạng thiết bị
        /// </summary>
        public int DeviceStatusID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public int? ManufacturerID { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int? SupplierID { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public int? CountryManufactureID { get; set; }

        /// <summary>
        /// Phân xưởng quản lý
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// Số SN
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// Chủng loại/ kiểu
        /// </summary>
        public string AssetType { get; set; }

        /// <summary>
        /// Thông số kỹ thuật
        /// </summary>
        public string Meter { get; set; }

        /// <summary>
        /// Mã kss
        /// </summary>
        public string Kss { get; set; }

        /// <summary>
        /// Ký hiệu vận hành
        /// </summary>
        public string Khvh { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Bản vẽ liên quan
        /// </summary>
        public string Drawing { get; set; }

        /// <summary>
        /// Hình ảnh thiết bị
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// Ngày lắp đặt
        /// </summary>
        public DateTime? InstallDate { get; set; }

        /// <summary>
        /// Ngày đưa vào vận hành
        /// </summary>
        public DateTime? OperatingDate { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        public int? CategoryID { get; set; }

        /// <summary>
        /// Link Google Map
        /// </summary>
        public string GoogleMapUrl { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
