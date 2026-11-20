using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin hoàn thành công tác điện
    /// </summary>
    public class LctdAddEditWorkFinishMobileModel
    {
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
        /// Thời gian hoàn thành lệnh công tác
        /// </summary>
        public string CompletedTime { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp (Người thi hành lệnh) đã báo cho ông (bà)
        /// </summary>
        public LctLeadershipUserSigningModel RepresentativeUnit { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy trực tiếp
        /// </summary>
        public LctLeadershipUserSigningModel Commander { get; set; }


    }
}
