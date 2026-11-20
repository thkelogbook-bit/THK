using Elogbook.Core.Enums;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số trả về
    /// </summary>
    public class ConfigWarningRuleResponseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Quy tắc
        /// </summary>
        public int RuleType { get; set; }

        /// <summary>
        /// Quy tắc
        /// </summary>
        public CwValueLabelModel RuleTypeInfo { get; set; }

        /// <summary>
        /// Quy tắc
        /// </summary>
        [JsonIgnore]
        public int RuleTypeValue
        {
            get
            {
                return RuleType;
            }
            set
            {
                RuleType = value;
                switch (value)
                {
                    case (int)DeviceErrorCode.Error:
                        RuleTypeInfo = new CwValueLabelModel { Value = value , Label = EnumExtensions.GetEnumDescription(EnumAiWarningRule.PersistentAnomalyDetection) } ;
                        break;
                    case (int)DeviceErrorCode.Stop:
                        RuleTypeInfo = new CwValueLabelModel { Value = value, Label = EnumExtensions.GetEnumDescription(EnumAiWarningRule.ParameterGroupAnomalyDetection) };
                        break;
                    case (int)DeviceErrorCode.Sim:
                        RuleTypeInfo = new CwValueLabelModel { Value = value, Label = EnumExtensions.GetEnumDescription(EnumAiWarningRule.ParameterTrendAnalysis) };
                        break;
                    default:
                        RuleTypeInfo = new CwValueLabelModel { Value = 0, Label = ""};
                        break;
                }
            }
        }

        /// <summary>
        /// Mức vượt ngưỡng (%) (thấp)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        public decimal ThresholdPercent1 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (trung bình)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        public decimal ThresholdPercent2 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (cao)
        /// </summary>
        public decimal ThresholdPercent3 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (thấp)
        /// </summary>
        public int ConsecutiveExceedCount1 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (trung bình)
        /// </summary>
        public int ConsecutiveExceedCount2 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (cao)
        /// </summary>
        public int ConsecutiveExceedCount3 { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Tham số trả về
    /// </summary>
    public class ConfigWarningResponseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số ngày
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// AiValueLabelModel
    /// </summary>
    public class CwValueLabelModel
    {
        /// <summary>
        /// Value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string Label { get; set; }
    }
}
