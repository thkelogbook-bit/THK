namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserPermission")]
    public partial class UserPermission : EntityBase
    {
        /// <summary>
        /// Định danh quyền chức năng
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PermissionKey { get; set; }

        /// <summary>
        /// Tên quyền chức năng
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PermissionName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Mã module
        /// </summary>
        [StringLength(255)]
        [Required]
        public string ModuleValue { get; set; }

        /// <summary>
        /// Module thuộc Admin Portal hay Mobile App
        /// </summary>
        [StringLength(50)]
        [Required]
        public string SystemType { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
