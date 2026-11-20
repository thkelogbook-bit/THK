namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Vai trò người dùng
    /// </summary>
    public class MobileUserRoleInfo : MobileEntityBaseModel
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
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Dữ liệu thao tác
        /// </summary>
        public int DataPermission { get; set; }

        /// <summary>
        /// Danh sách quyền của Vai trò
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Xác định vai trò Admin
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
