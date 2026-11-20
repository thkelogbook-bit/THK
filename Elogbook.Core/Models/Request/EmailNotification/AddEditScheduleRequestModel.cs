using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một schedule
    /// </summary>
    public class AddEditScheduleRequestModel
    {
        /// <summary>
        /// Mã schedule
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string ScheduleContent { get; set; }

        /// <summary>
        /// Thời điểm thực hiện công việc
        /// </summary>
        public string ScheduleValue { get; set; }

        /// <summary>
        /// Mô tả chi tiết thời gian cấu hình schedule
        /// </summary>
        public string ScheduleDescription { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Dữ liệu mặc định hệ thống, không được cập nhật
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Send email khi run schedule
        /// </summary>
        public bool IsSendEmail { get; set; }

        /// <summary>
        /// Gửi tới
        /// </summary>
        public string EmailTo { get; set; }

        /// <summary>
        /// CC những người liên quan
        /// </summary>
        public string EmailCC { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Nội dung email
        /// </summary>
        public string EmailContent { get; set; }

        /// <summary>
        /// ID cấu hình SLA
        /// </summary>
        public int? SlaConfigID { get; set; }

        /// <summary>
        /// ID danh mục SLA
        /// </summary>
        public int? SlaCategoryID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
