using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Vai trò SLA
    /// </summary>
    [Table("SlaRoles")]
    public partial class SlaRole : EntityBase
    {
        /// <summary>
        /// Tên viết tắt vai trò SLA (e.g., 'CHTT', 'NCP')
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đầy đủ vai trò SLA (e.g., 'Chỉ huy trực tiếp', 'Người cho phép')
        /// </summary>
        public string Description { get; set; }
    }
}
