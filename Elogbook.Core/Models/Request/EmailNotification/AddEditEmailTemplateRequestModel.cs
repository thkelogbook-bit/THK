using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một email template
    /// </summary>
    public class AddEditEmailTemplateRequestModel
    {
        /// <summary>
        /// Mã email template
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Loại sự kiện gửi email
        /// </summary>
        public int NotificationTypeID { get; set; }

        /// <summary>
        /// Các chức năng có sự kiện gửi email
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Nội dung email
        /// </summary>
        public string EmailContent { get; set; }

        /// <summary>
        /// Mã email template
        /// </summary>
        public string EmailCode { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
