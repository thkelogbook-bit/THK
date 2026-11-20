namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// thêm 1 người quản lý công tác
    /// </summary>
    public class AddNewLeadershipUserFetchedModel
    {
        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }
        /// <summary>
        /// ID User
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Vai trò của người thamg giam
        /// </summary>
        public int? Roles { get; set; }

        /// <summary>
        /// cấp bật ATĐ
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Nhiệm vụ
        /// </summary>
        public string Mission { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }
    }
}
