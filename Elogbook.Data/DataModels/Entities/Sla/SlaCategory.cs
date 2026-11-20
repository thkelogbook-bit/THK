using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Phân loại SLA (e.g., 'PCT', 'PTT', 'PYC')
    /// </summary>
    [Table("SlaCategories")]
    public partial class SlaCategory : EntityBase
    {
        /// <summary>
        /// Tên viết tắt phân loại SLA (e.g., 'PCT', 'PTT', 'PYC')
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đầy đủ phân loại SLA
        /// </summary>
        public string Description { get; set; }
    }
}
