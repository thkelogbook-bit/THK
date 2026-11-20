using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình cảnh báo AI
    /// </summary>
    [Table("ConfigurationWarningRules")]
    public partial class ConfigurationWarningRule : EntityBase
    {
        /// <summary>
        /// Cấu hình chung ID
        /// </summary>
        public int ConfigurationWarningID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ConfigurationWarningID))]
        public virtual ConfigurationWarning ConfigurationWarning { get; set; }

        /// <summary>
        /// Quy tắc
        /// </summary>
        public int RuleType { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (thấp)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        [Column(TypeName = "decimal")]
        public decimal ThresholdPercent1 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (trung bình)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        [Column(TypeName = "decimal")]
        public decimal ThresholdPercent2 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (cao)
        /// </summary>
        [Column(TypeName = "decimal")]
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
}
