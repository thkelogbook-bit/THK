using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("UserGroups")]
    public partial class UserGroup : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Tên của nhóm người dùng
        /// </summary>
        public string UserGroupName { get; set; }

        /// <summary>
        /// Tài khoản người dùng
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<UserInGroup> UserInGroups { get; set; }

        /// <summary>
        /// Trạng thái sử dụng (True: được sử dụng, False: Không được sử dụng)
        /// </summary>
        public bool Active { get; set; }
    }
}
