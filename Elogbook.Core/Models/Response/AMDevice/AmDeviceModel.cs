using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số thuộc tính của một thiết bị
    /// </summary>
    public class AmDeviceModel : EntityBaseModel
    {
        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string AssetDesc { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public ComboboxDataModel TypeDesc { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public ComboboxDataModel Manufacturer { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public ComboboxDataModel Supplier { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public ComboboxDataModel CountryManufacture { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public ValueLabelCheckedModel Organization { get; set; }

        /// <summary>
        /// Thiết bị cha
        /// </summary>
        [JsonIgnore]
        public string AssetParentStr { get; set; }

        /// <summary>
        /// Thiết bị cha
        /// </summary>
        public ComboboxDataModel AssetParent { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public ComboboxDataModel SiteDesc { get; set; }

        /// <summary>
        /// Tình trang thiết bị
        /// </summary>
        public ValueLabelCheckedModel DeviceStatus { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Áp dụng, 2: Không áp dụng
        /// </summary>
        [JsonIgnore]
        public int Active
        {
            get
            {
                return _statusValue;
            }
            set
            {
                _statusValue = value;
                switch (_statusValue)
                {
                    case 1:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply) };
                        break;
                    case 2:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply) };
                        break;
                    default:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel Status { get; set; }

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
        [JsonIgnore]
        public string ListImageIds { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> Images { get; set; }

        /// <summary>
        /// Ngày lắp đặt
        /// </summary>
        public DateTime? InstallDate { get; set; }

        /// <summary>
        /// Ngày đưa vào vận hành
        /// </summary>
        public DateTime? OperatingDate { get; set; }

        /// <summary>
        /// Danh sách thiết bị con
        /// </summary>
        [JsonIgnore]
        public List<AmDeviceModel> AssetChilds { get; set; }

        /// <summary>
        /// Phân loại thiết bị
        /// </summary>
        public ComboboxDataModel DeviceCategory { get; set; }

        /// <summary>
        /// Link Google Map
        /// </summary>
        public string GoogleMapUrl { get; set; }

        /// <summary>
        /// Có thiết bị con hay không
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasChildren { get; set; }
    }

    /// <summary>
    /// Thông tin chi tiết thiết bị
    /// </summary>
    public class AmDeviceBasicInfoModel
    {
        /// <summary>
        /// ID thiết bị
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string AssetDesc { get; set; }

        /// <summary>
        /// Mã thiết bị
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// Thiết bị cha
        /// </summary>
        public string AssetParent { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public ValueLabelCheckedModel TypeDesc { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public ValueLabelCheckedModel SiteDesc { get; set; }

        /// <summary>
        /// Tình trang thiết bị
        /// </summary>
        public ValueLabelCheckedModel DeviceStatus { get; set; }

        /// <summary>
        /// Nhà chế tạo
        /// </summary>
        public ValueLabelCheckedModel Manufacturer { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public ValueLabelCheckedModel Supplier { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public ValueLabelCheckedModel CountryManufacture { get; set; }
    }
}
