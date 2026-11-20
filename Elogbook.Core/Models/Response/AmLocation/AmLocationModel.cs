using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin địa điểm
    /// </summary>
    public class AmLocationModel : EntityBaseModel
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public ValueLabelCheckedModel Organization { get; set; }

        /// <summary>
        /// Tên khu vực
        /// </summary>
        public string SiteDesc { get; set; }

        /// <summary>
        /// Mã Khu vực
        /// </summary>
        public string SiteCode { get; set; }


        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Có thể sử dụng, 2: Không thể sử dụng 
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
        /// Khu vực cha
        /// </summary>
        [JsonIgnore]
        public string SiteParentStr { get; set; }

        /// <summary>
        /// Khu vực cha
        /// </summary>
        public ComboboxDataModel SiteParent { get; set; }

        /// <summary>
        /// Danh sách khu vực con
        /// </summary>
        [JsonIgnore]
        public List<AmLocationModel> SiteChilds { get; set; }
    }
}
