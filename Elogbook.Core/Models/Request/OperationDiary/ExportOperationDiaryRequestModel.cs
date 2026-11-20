using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Export dữ liệu nhật ký vận hành
    /// </summary>
    public class ExportOperationDiaryRequestModel
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
        /// Ngày tạo nhật ký từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày tạo nhật ký đến ngày
        /// </summary>
        public string ToDate { get; set; }

        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
