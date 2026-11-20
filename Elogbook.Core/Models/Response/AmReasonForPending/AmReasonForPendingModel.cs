using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết Nguyên nhân tạm dừng công việc
    /// </summary>
    public class AmReasonForPendingModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Áp dụng, 2: ko Áp dụng
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
