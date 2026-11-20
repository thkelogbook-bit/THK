namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Vai trò của Nhân viên
    /// </summary>
    public class MobileRoleInUsersInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Vai trò
        /// </summary>
        public int UserRoleID { get; set; }

        /// <summary>
        /// Vai trò thuộc người dùng nào
        /// </summary>
        public int UserID { get; set; }
    }
}
