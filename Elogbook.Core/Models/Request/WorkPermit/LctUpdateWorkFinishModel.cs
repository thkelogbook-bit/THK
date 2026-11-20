using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để xác nhận hoàn thành lệnh công tác
    /// </summary>
    public class LctUpdateWorkFinishModel
    {
        /// Id của lệnh công tác
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
