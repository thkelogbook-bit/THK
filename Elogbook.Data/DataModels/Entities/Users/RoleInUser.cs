namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RoleInUsers")]
    public partial class RoleInUser : EntityBase
    {
        /// <summary>
        /// Vai trò
        /// </summary>
        public int UserRoleID { get; set; }

        [ForeignKey(nameof(UserRoleID))]
        [MapperIgnore]
        public virtual UserRole UserRole { get; set; }

        /// <summary>
        /// Vai trò thuộc người dùng nào
        /// </summary>
        public int UserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }
    }
}
