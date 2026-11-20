using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết Nguyên nhân dừng thực hiện
    /// </summary>
    public class AmReasonForStopModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

        private int _typeValue;
        /// <summary>
        /// Trạng thái 1: Không thực hiện AM, 2: Dừng công việc
        /// </summary>
        [JsonIgnore]
        public int Type
        {
            get
            {
                return _typeValue;
            }
            set
            {
                _typeValue = value;
                switch (_typeValue)
                {
                    case 1:
                        ReasonType = new ValueLabelCheckedModel { Value = _typeValue, Label = EnumExtensions.GetEnumDescription(AmEnumReasonType.StopAM) };
                        break;
                    case 2:
                        ReasonType = new ValueLabelCheckedModel { Value = _typeValue, Label = EnumExtensions.GetEnumDescription(AmEnumReasonType.StopWork) };
                        break;
                    default:
                        ReasonType = new ValueLabelCheckedModel { Value = _typeValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Loại
        /// </summary>
        public ValueLabelCheckedModel ReasonType { get; set; }

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
    }
}
