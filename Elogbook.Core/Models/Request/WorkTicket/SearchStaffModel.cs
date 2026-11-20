namespace Elogbook.Core.Models.Request.WorkTicket
{
    /// <summary>
    /// Tham số tìm kiếm nhân viên
    /// </summary>
    public class SearchStaffModel
    {
        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Thuộc quyền quản lý (-1 Tất cả, 1: Trong công ty, 2: Ngoài công ty)
        /// </summary>
        public int TypeStaff { get; set; }

        /// <summary>
        /// Nhập Họ và tên or số thẻ ATĐ
        /// </summary>
        public string SearchText { get; set; }

    }
}
