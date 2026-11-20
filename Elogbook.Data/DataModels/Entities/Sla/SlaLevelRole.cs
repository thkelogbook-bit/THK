using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Bảng ánh xạ vai trò cho cấp độ SLA
    /// </summary>
    [Table("SlaLevelRoles")]
    public partial class SlaLevelRole : EntityBase
    {
        /// <summary>
        /// Cấp độ SLA liên quan
        /// </summary>
        public int SlaLevelID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(SlaLevelID))]
        public virtual SlaLevel SlaLevel { get; set; }

        /// <summary>
        /// Vai trò SLA liên quan
        /// </summary>
        public int SlaRoleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(SlaRoleID))]
        public virtual SlaRole SlaRole { get; set; }
    }
}
