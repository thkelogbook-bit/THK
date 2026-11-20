using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Điều kiện tìm kiếm thông tin sửa chữa lớn
    /// </summary>
    public class MajorRepairSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên đợt sửa chữa lớn
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Kế hoạch thực hiện (bắt đầu)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Kế hoạch thực hiện (kết thúc)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Ngày tạo (From)
        /// </summary>
        public string CreatedFromDate { get; set; }

        /// <summary>
        /// Ngày tạo (To)
        /// </summary>
        public string CreatedToDate { get; set; }

        /// <summary>
        /// Trạng thái các đợt sửa chữa lớn 
        /// </summary>
        public int MajorRepairStatusID { get; set; }
    }
}
