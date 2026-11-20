using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết của công việc
    /// </summary>
    public class AmWorkModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public ValueLabelCheckedModel WorkType { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string WorkDesc { get; set; }

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
