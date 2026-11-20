using Elogbook.Core.Models.Request.EmailNotification;
using Elogbook.Core.Models.Response.EmailNotification;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức sử dụng cho cấu hình email hệ thống
    /// </summary>
    public interface IEmailConfigurationService
    {
        /// <summary>
        /// Lấy thông tin cấu hình email hệ thống
        /// </summary>
        /// <returns></returns>
        EmailConfigurationResponseModel GetEmailConfiguration();

        /// <summary>
        /// Lưu thông tin email hệ thống
        /// </summary>
        /// <returns></returns>
        EmailConfigurationResponseModel Save(EmailConfigurationRequestModel requestModel);
    }
}
