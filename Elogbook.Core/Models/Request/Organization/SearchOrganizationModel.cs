namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Quản ký tổ chức
    /// </summary>
    public class SearchOrganizationModel
    {
        /// <summary>
        /// Tên Công ty, Phân xưởng, Tổ, Đội/Nhóm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public int Active { get; set; }
    }
}
