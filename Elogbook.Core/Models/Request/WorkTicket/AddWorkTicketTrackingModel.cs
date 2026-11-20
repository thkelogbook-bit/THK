using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// WorkTicketTracking Model Request
    /// </summary>
    public class AddWorkTicketTrackingModel
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Mã số của Người yêu cầu thay đổi
        /// </summary>
        public int RequesterUserID { get; set; }

        /// <summary>
        /// Mã số của Người cho phép cần thay đổi
        /// </summary>
        public int NewAllowerUserID { get; set; }

        /// <summary>
        /// Lý do yêu cầu thay đổi người cho phép ()
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
