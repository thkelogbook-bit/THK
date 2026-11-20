using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// UpdateAlertNoticeModel
    /// </summary>
    public class UpdateAlertNoticeModel
    {
        /// <summary>
        /// ID thiết bị
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsImportant { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
