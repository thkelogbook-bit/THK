using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Điều kiện SLA (Trước hạn / Đến hạn / Quá hạn)
    /// </summary>
    [Table("SlaConditions")]
    public partial class SlaCondition : EntityBase
    {
        /// <summary>
        /// Tên điều kiện SLA (Trước hạn / Đến hạn / Quá hạn)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả (nếu có)
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
