using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh mục - Thiết bị
    /// </summary>
    public class MobileAmDevice : MobileEntityBaseModel
    {
        /// <summary>
        /// Khu vực
        /// </summary>
        public int? AmLocationID { get; set; }

        /// <summary>
        /// Thiết bị cha
        /// </summary>
        public string AssetParent { get; set; }

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string AssetDesc { get; set; }

        /// <summary>
        /// Mã thiết bị
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// Tình trạng thiết bị
        /// </summary>
        public int? AmDeviceStatusID { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public int? AmDeviceTypeID { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public int? AmManufacturerID { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int? AmSupplierID { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public int? AmCountryManufactureID { get; set; }

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
        public int? AmDeviceCategoryID { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }
    }
}
