using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Khung giờ
    /// </summary>
    public  class SearchOperationParametersModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int OperationTimeLineID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int ParameterGroupID { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public string SearchDate { get; set; }

        /// <summary>
        /// Loại lấy kết quả (0.Thông số chưa ghi, 1.Thông số đã được ghi,  -1.Tất cả)
        /// </summary>
        public int TypeGet { get; set; }
    }
}
