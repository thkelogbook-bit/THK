namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Schedule")]
    public partial class Schedule : EntityBase
    {
        /// <summary>
        /// Tên công việc
        /// </summary>
        [Required]
        [StringLength(500)]
        public string ScheduleName { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        [StringLength(500)]
        public string ScheduleContent { get; set; }

        /// <summary>
        /// Thời gian thực hiện công việc
        /// </summary>
        [StringLength(500)]
        public string ScheduleValue { get; set; }

        /// <summary>
        /// Mô tả chi tiết thời gian cấu hình schedule
        /// </summary>
        [StringLength(1000)]
        public string ScheduleDescription { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        [Required]
        public int Active { get; set; }

        /// <summary>
        /// Dữ liệu mặc định hệ thống, không được cập nhật
        /// </summary>
        [Required]
        public bool IsSystem { get; set; }

        /// <summary>
        /// Send email khi run schedule
        /// </summary>
        [Required]
        public bool IsSendEmail { get; set; }

        /// <summary>
        /// Gửi tới
        /// </summary>
        [StringLength(500)]
        public string EmailTo { get; set; }

        /// <summary>
        /// CC những người liên quan
        /// </summary>
        [StringLength(500)]
        public string EmailCC { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        [StringLength(500)]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Nội dung email
        /// </summary>
        public string EmailContent { get; set; }

        /// <summary>
        /// SLA
        /// </summary>
        public int? SlaConfigID { get; set; }
        [ForeignKey(nameof(SlaConfigID))]
        [MapperIgnore]
        public virtual SlaConfig SlaConfig { get; set; }

        /// <summary>
        /// ID phân loại SLA liên quan
        /// </summary>
        public int? SlaCategoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(SlaCategoryID))]
        public virtual SlaCategory SlaCategory { get; set; }
    }
}
