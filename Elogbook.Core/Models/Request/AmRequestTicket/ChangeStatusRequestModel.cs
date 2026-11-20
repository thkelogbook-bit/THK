using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// request change status PYC
    /// </summary>
    public class ChangeStatusRequestModel
    {
        /// <summary>
        /// mã số phiếu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// trạng thái phiếu
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Loại phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketType { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
