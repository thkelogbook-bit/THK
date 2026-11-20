using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cần đề tạo file PDF
    /// </summary>
    public class PdfExportOPModel
    {
        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Ngày ghi thông số vận hành
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
