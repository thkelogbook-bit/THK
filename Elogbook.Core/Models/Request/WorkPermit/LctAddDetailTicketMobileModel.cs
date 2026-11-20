using Elogbook.Core.Models.Request.InspectionReport;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để cập nhật chi tiết lệnh công tác
    /// </summary>
    public class LctAddDetailTicketMobileModel
    {
        /// <summary>
        /// Danh sách hình ảnh công tác
        /// </summary>
        public List<string> WorkPictures { get; set; }

        /// <summary>
        /// Kết quả công tác hàng ngày
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy trực tiếp
        /// </summary>
        public LctLeadershipUserSigningModel Commander{ get; set; }
    }
}
