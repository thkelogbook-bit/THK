using Elogbook.Core.Enums;
using Elogbook.Core.Models.BaseFilter;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số xử lý gửi email
    /// </summary>
    public class ProcessEmailRequestModel
    {
        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Mã sự kiện gửi email
        /// </summary>
        public int NotificationTypeValue { get; set; }

        /// <summary>
        /// Chức năng thực hiện thông báo
        /// </summary>
        public EnumModuleNotification ModuleNotification { get; set; }

        /// <summary>
        /// Loại phiếu công tác
        /// </summary>
        public int RegistrationTicketType { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int TicketStatus { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// có phải phiếu công tác không
        /// </summary>
        public bool IsPCT { get; set; }

        /// <summary>
        /// Mã phiếu yêu cầu
        /// </summary>
        public int AmRequestTicketID { get; set; }

        /// <summary>
        /// Mã nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Thời gian sớm/trễ
        /// </summary>
        public string TimeDue { get; set; }

        /// <summary>
        /// Danh sách nhóm người dùng
        /// </summary>
        public List<int> UserGroupIds { get; set; }
    }
}
