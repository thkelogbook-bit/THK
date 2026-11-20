using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("UserInGroup")]
    public partial class UserInGroup :EntityBase
    {
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        [Required]
        public int UserGroupsID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(UserGroupsID))]
        public virtual UserGroup UserGroups { get; set; }

        /// <summary>
        /// Người dùng trong hệ thống
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// Loại người dùng (True: Người ngoài hệ thống, False: Người nội bộ )
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
