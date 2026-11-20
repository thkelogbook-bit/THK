using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một nhân viên công tác
    /// </summary>
    public class LctAddEditWorkerInformationModel
    {
        /// <summary>
        /// lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Tên công nhân
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        public string CheckInTime { get; set; }

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public SignatureModel CheckInSignature { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public string CheckOutTime { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public SignatureModel CheckOutSignature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tài khoản người dùng nội bộ/ngoài hệ thống
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Xác định người dùng nội bộ/ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRankStr { get; set; }

    }
}
