namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// thông tin người tham gia phiếu thao tác
    /// </summary>
    public class UserInfoInTicketModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public int Roles { get; set; }
        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Signature
        /// </summary>
        public SignatureModel Signature { get; set; }
    }
}
