using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Phần xưởng
    /// </summary>
    public class SearchProductionPlantModel : BaseListFilter
    {
        /// <summary>
        /// Tên phân xưởng
        /// </summary>
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
