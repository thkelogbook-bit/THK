namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin email template
    /// </summary>
    public class EmailTemplateResponseModel : EntityBaseModel
    {
        /// <summary>
        /// Mã loại sự kiện gửi email
        /// </summary>
        public int NotificationTypeID { get; set; }

        /// <summary>
        /// Thông tin loại sự kiện gửi email
        /// </summary>
        public ValueLabelCheckedModel NotificationType { get; set; }

        /// <summary>
        /// Chức năng gửi thông báo
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// Chức năng sử dụng email
        /// </summary>
        public ValueLabelCheckedModel Module { get; set; }

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
        public ValueLabelCheckedModel Factory { get; set; }
    }
}
