namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// người quản lý ký
    /// </summary>
    public class LeadershipUserSigningModel
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Chữ ký
        /// </summary>
        public SignatureModel Signature { get; set; }

        /// <summary>
        /// Loại user trong hay ngoài hệ thống
        /// </summary>
        public bool? IsExternal { get; set; }
    }
}
