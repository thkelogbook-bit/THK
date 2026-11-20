namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin schedule
    /// </summary>
    public class ScheduleResponseModel : EntityBaseModel
    {
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
        /// SLA configuration information
        /// </summary>
        public ScheduleSlaConfigModel SlaConfig { get; set; }

        /// <summary>
        /// SLA category information
        /// </summary>
        public ScheduleSlaCategoryModel SlaCategory { get; set; }
    }

    /// <summary>
    /// Model for SLA configuration information
    /// </summary>
    public class ScheduleSlaConfigModel
    {
        /// <summary>
        /// ID cấu hình SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name of the SLA configuration
        /// </summary>
        public string SlaName { get; set; }

        /// <summary>
        /// Description of the SLA configuration
        /// </summary>
        public string SlaDescription { get; set; }

        /// <summary>
        /// Status of the SLA configuration
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Model for ScheduleSlaCategoryModel
    /// </summary>
    public class ScheduleSlaCategoryModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SlaCategoryName
        /// </summary>
        public string SlaCategoryName { get; set; }

        /// <summary>
        /// SlaCategoryDescription
        /// </summary>
        public string SlaCategoryDescription { get; set; }
    }
}
