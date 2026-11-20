using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết của Loại công việc
    /// </summary>
    public class AmWorkTypeModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string TypeDesc { get; set; }

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
    }
}
