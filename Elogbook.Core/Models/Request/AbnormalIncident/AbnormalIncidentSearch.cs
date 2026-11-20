using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Nội dung xử lý bất thường sự cố
    /// </summary>
    public class AbnormalIncidentSearch : BaseListFilter
    {
        /// <summary>
        /// year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        public int Month { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Các Month
        /// </summary>
        public string Months { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;
        
        /// <summary>
        /// Danh sách Nhà máy
        /// </summary>
        public string FactoryIDs { get; set; }

        /// <summary>
        /// Title export excel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Báo cáo gửi mail
        /// </summary>
        [JsonIgnore]
        public bool IsMailReport { get; set; } = false;
    }
}
