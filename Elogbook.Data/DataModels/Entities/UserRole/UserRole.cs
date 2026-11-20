namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserRoles")]
    public partial class UserRole : EntityBase
    {
        /// <summary>
        /// Tên vai trò
        /// </summary>
        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }

        /// <summary>
        /// Mô tả chi tiết vai trò
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Phạm vi sử dụng
        /// </summary>
        public int DataPermission { get; set; }

        /// <summary>
        /// Danh sách quyền của người dùng trong vai trò
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Xác định vai trò Admin
        /// </summary>
        public bool IsAdmin { get; set; }

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
