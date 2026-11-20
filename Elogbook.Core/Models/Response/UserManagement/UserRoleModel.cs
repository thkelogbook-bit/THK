using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Vai trò người dùng
    /// </summary>
    public class UserRoleModel : EntityBaseModel
    {
        /// <summary>
        /// Tên vai trò
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Quyền thao tác
        /// </summary>
        public int DataPermission { get; set; }

        /// <summary>
        /// Quyền người dùng
        /// </summary>
        [JsonIgnore]
        public string Permissions { get; set; }

        /// <summary>
        /// Danh sách quyền người dùng
        /// </summary>
        public List<UserPermissionModel> UserPermissions { get; set; }

        /// <summary>
        /// Xác định vai trò Admin
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
