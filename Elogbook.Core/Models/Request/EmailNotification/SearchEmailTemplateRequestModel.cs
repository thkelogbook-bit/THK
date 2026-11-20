using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm email template
    /// </summary>
    public class SearchEmailTemplateRequestModel : BaseListFilter
    {
        /// <summary>
        /// Loại sự kiện gửi email
        /// </summary>
        public int NotificationTypeID { get; set; } = -1;

        /// <summary>
        /// Các chức năng có sự kiên gửi email
        /// </summary>
        public int ModuleID { get; set; } = -1;

        /// <summary>
        /// Chủ đề email
        /// </summary>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; } = -1;

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; } = -1;
    }
}
