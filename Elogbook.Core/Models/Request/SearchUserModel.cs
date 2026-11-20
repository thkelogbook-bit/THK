using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm người dùng
    /// </summary>
    public class SearchUserModel : BaseListFilter
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int? ProductionPlantID { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Họ và tên/Tên đăng nhập
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public int? AccountType { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public int? UserRoleID { get; set; }

        /// <summary>
        /// Nguồn tài khoản(AD hoặc thêm từ portal)
        /// </summary>
        public int? SourceType { get; set; }
    }
}
