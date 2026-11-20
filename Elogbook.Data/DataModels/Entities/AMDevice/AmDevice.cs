using Elogbook.Core.Enums;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Danh mục - Thiết bị
    /// </summary>
    [Table("AmDevice")]
    public partial class AmDevice : EntityBase
    {
        /// <summary>
        /// Khu vực
        /// </summary>
        public int? AmLocationID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmLocationID))]
        public virtual AmLocation AmLocation { get; set; }

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
        public int AmDeviceStatusID { get; set; } = 1;

        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceStatusID))]
        public virtual AmDeviceStatus AmDeviceStatus { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public int? AmDeviceTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceTypeID))]
        public virtual AmDeviceType AmDeviceType { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public int? AmManufacturerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmManufacturerID))]
        public virtual AmManufacturer AmManufacturer { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int? AmSupplierID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmSupplierID))]
        public virtual AmSupplier AmSupplier { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public int? AmCountryManufactureID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmCountryManufactureID))]
        public virtual AmCountryManufacture AmCountryManufacture { get; set; }

        /// <summary>
        /// Phân xưởng quản lý
        /// </summary>
        public int? OrganizationID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OrganizationID))]
        public virtual Organization Organization { get; set; }

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

        [MapperIgnore]
        [ForeignKey(nameof(AmDeviceCategoryID))]
        public virtual AmDeviceCategory AmDeviceCategory { get; set; }

        /// <summary>
        /// PMISId
        /// </summary>
        public string PMISId { get; set; }

        /// <summary>
        /// Link Google Map
        /// </summary>
        public string GoogleMapUrl { get; set; }


    }
}
