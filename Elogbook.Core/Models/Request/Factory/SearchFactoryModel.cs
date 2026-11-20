using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm nhà máy
    /// </summary>
    public class SearchFactoryModel : BaseListFilter
    {
        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int ProductionPlantID { get; set; }
    }
}
