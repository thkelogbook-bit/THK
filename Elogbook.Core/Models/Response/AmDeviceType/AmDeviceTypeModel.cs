using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Loại thiết bị
    /// </summary>
    public class AmDeviceTypeModel : EntityBaseModel
    {
        /// <summary>
        /// Tên loại thiết bị
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string TypeCode { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Áp dụng, 2: Không Áp dụng
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
    }
}
