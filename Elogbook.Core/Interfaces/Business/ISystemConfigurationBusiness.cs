using Elogbook.Core.Models.Request.SystemConfiguration;
using Elogbook.Core.Models.Response.SystemConfiguration;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức sử dụng cho cấu hình hệ thống
    /// </summary>
    public interface ISystemConfigurationBusiness
    {
        /// <summary>
        /// Lấy thông tin version mobile app
        /// </summary>
        /// <returns></returns>
        AppVersionModel GetAppVersion();

        /// <summary>
        /// Lấy thông tin version mobile app
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
