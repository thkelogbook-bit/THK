using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Thông số
    /// </summary>
    public class SearchParameterModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Thuộc nhóm thông số
        /// </summary>
        public int ParameterType { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int ParameterGroupID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycelID { get; set; }
    }
}
