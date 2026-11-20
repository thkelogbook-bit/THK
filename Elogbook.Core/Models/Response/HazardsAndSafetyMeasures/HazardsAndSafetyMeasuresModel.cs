using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chi tiết
    /// </summary>
    public class HazardsAndSafetyMeasuresModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string ContentText { get; set; }

        private int _contentType;
        /// <summary>
        /// Loại 1: Mối nguy hiểm, 2: BIện pháp an toàn
        /// </summary>
        [JsonIgnore]
        public int TypeValue
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
                switch (_contentType)
                {
                    case 1:
                        ContentType = new ValueLabelCheckedModel { Value = _contentType, Label = EnumExtensions.GetEnumDescription(EnumHazardsAndSafetyMeasures.Hazards) };
                        break;
                    case 2:
                        ContentType = new ValueLabelCheckedModel { Value = _contentType, Label = EnumExtensions.GetEnumDescription(EnumHazardsAndSafetyMeasures.SafetyMeasures) };
                        break;
                    default:
                        ContentType = new ValueLabelCheckedModel { Value = _contentType, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Loại
        /// </summary>
        public ValueLabelCheckedModel ContentType { get; set; }

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
