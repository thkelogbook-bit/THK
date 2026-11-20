using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để xác nhận hoàn thành phiếu công tác
    /// </summary>
    public class UpdateWorkFinishModel
    {
        /// Id của phiếu công tác
        public int WorkTicketID { get; set; }

        /// Ngày hoàn thành
        public string FinishDate { get; set; }

        /// loại chữ ký
        public int SignatureType { get; set; }

        /// Người dùng hiện tại
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
