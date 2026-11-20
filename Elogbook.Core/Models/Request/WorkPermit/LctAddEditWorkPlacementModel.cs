using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin địa điểm làm việc
    /// </summary>
    public class LctAddEditWorkPlacementModel
    {
        /// <summary>
        /// lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Địa điểm công tác
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public SignatureModel CommanderSignature { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public SignatureModel AllowerSignature { get; set; }

        /// <summary>
        /// Điều kiện an toàn
        /// </summary>
        public string SafeCondition { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
