using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số để xem thông số vận hành
    /// </summary>
    public class ViewOperationParametersModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public string SearchDate { get; set; }
    }
}
