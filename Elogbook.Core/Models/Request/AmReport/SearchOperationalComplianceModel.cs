using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// khai báo model search tổng quan báo cáo tuân thủ hoạt động
    /// </summary>
    public class SearchOperationalComplianceModel : BaseListFilter
    {
        /// <summary>
        /// nhà máy
        /// </summary>
        public int FactoryId { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// tháng
        /// </summary>
        public int Month { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// trạng thái
        /// </summary>
        public int Status { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Danh sách nhà máy
        /// </summary>
        public string FactoryIDs { get; set; }

        /// <summary>
        /// Danh sách tháng
        /// </summary>
        public string Months { get; set; }

        /// <summary>
        /// Báo cáo gửi mail
        /// </summary>
        [JsonIgnore]
        public bool IsMailReport { get; set; } = false;
    }
}
