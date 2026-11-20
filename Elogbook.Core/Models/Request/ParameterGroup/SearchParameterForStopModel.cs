namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Thông số
    /// </summary>
    public class SearchParameterForStopModel 
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
        /// Nhóm thông số
        /// </summary>
        public int ParameterGroupID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycelID { get; set; }
    }
}
