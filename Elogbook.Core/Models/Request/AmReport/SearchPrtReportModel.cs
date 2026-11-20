using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Điều kiện tìm kiếm
    /// </summary>
    public class SearchPrtReportModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public int EmergencyID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public int ProcessingStatusID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int StatusID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Danh sách nhà máy
        /// </summary>
        public string FactoryIDs { get; set; }

        /// <summary>
        /// Danh sách tháng
        /// </summary>
        public string Months { get; set; }

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
