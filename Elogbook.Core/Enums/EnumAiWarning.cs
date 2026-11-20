using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Quy tắc cảnh báo
    /// </summary>
    public enum EnumAiWarningRule
    {
        /// <summary>
        /// Phát hiện bất thường kéo dài
        /// </summary>
        [Description("Phát hiện bất thường kéo dài")]
        PersistentAnomalyDetection = 1,

        /// <summary>
        /// Phát hiện bất thường theo nhóm thông số
        /// </summary>
        [Description("Phát hiện bất thường theo nhóm thông số")]
        ParameterGroupAnomalyDetection,

        /// <summary>
        /// Phân tích xu hướng biến động thông số
        /// </summary>
        [Description("Phân tích xu hướng biến động thông số")]
        ParameterTrendAnalysis
    }

    /// <summary>
    /// Mức độ cảnh báo
    /// </summary>
    public enum EnumAiWarningLever
    {
        /// <summary>
        /// Thấp
        /// </summary>
        [Description("Thấp")]
        Low = 1,
        /// <summary>
        /// Trung bình
        /// </summary>
        [Description("Trung bình")]
        Medium,
        /// <summary>
        /// Cao
        /// </summary>
        [Description("Cao")]
        High,
    }
}
