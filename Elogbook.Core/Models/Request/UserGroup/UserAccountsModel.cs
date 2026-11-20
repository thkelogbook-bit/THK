namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tài khoản người dùng
    /// </summary>
    public class UserAccountsModel
    {
        /// <summary>
        /// ID Người tài khoản người dùng
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Loại người dùng (True: Người ngoài hệ thống, False: Người nội bộ )
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// Loại cập nhập (True: Insert, False: Delete)
        /// </summary>
        public bool IsInsert { get; set; }
    }
}
