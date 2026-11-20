using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Vai trò
    /// </summary>
    public class AddEditUserRoleModel
    {
        /// <summary>
        /// Mã vai trò
        /// </summary>
        public int ID { get; set; }

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
        /// Danh sách quyền người dùng
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Xác định vai trò Admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
