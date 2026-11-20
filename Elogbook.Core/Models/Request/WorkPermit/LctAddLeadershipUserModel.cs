namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// thêm 1 người quản lý công tác
    /// </summary>
    public class LctAddLeadershipUserModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
