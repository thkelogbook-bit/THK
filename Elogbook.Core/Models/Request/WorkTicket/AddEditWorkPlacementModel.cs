using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin địa điểm làm việc
    /// </summary>
    public class AddEditWorkPlacementModel
    {
        /// <summary>
        /// Phiếu công tác
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
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký bắt đầu xác nhận bắt đầu làm việc của người chỉ huy trực tiếp
        /// </summary>
        public SignatureModel CommanderSignature { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký bắt đầu xác nhận bắt đầu làm việc của người cho phép
        /// </summary>
        public SignatureModel AllowerSignature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
