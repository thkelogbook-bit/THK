using Elogbook.Core.Models.Request.SystemConfiguration;
using Elogbook.Core.Models.Response.SystemConfiguration;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức sử dụng cho cấu hình hệ thống
    /// </summary>
    public interface ISystemConfigurationService
    {
        /// <summary>
        /// Lấy thông tin version mobile app
        /// </summary>
        /// <returns></returns>
        AppVersionModel GetAppVersion();

        /// <summary>
        /// Lưu version mobile app
        /// </summary>
        /// <returns></returns>
        AppVersionModel Save(AppVersionInputModel requestModel);

        /// <summary>
        /// Lấy thông tin cấu hình hệ thống
        /// </summary>
        /// <returns></returns>
        SystemConfigurationModel GetSystemConfiguration();

        /// <summary>
        /// Cập nhật cấu hình hệ thống
        /// </summary>
        /// <returns></returns>
        SystemConfigurationModel UpdateSystemConfiguration(UpdateSystemConfigurationModel requestModel);
    }
}
