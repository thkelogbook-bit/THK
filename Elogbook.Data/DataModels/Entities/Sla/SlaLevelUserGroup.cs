using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Data
{
    /// <summary>
    /// Bảng ánh xạ vai trò cho cấp độ SLA
    /// </summary>
    [Table("SlaLevelUserGroups")]
    public partial class SlaLevelUserGroup : EntityBase
    {
        /// <summary>
        /// Cấp độ SLA liên quan
        /// </summary>
        public int SlaLevelID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(SlaLevelID))]
        public virtual SlaLevel SlaLevel { get; set; }

        /// <summary>
        /// Nhóm người dùng liên quan
        /// </summary>
        [Required]
        public int UserGroupsID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(UserGroupsID))]
        public virtual UserGroup UserGroups { get; set; }
    }
}
