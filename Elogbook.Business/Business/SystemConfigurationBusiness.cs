using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request.SystemConfiguration;
using Elogbook.Core.Models.Response.SystemConfiguration;
using System;

namespace Elogbook.Business
{
    /// <summary>
    /// Xử lý các phương thức sử dụng cho cấu hình hệ thống
    /// </summary>
    public class SystemConfigurationBusiness : ISystemConfigurationBusiness
    {
        private readonly ISystemConfigurationService _systemConfigurationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public SystemConfigurationBusiness(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService = systemConfigurationService;
        }

        public AppVersionModel GetAppVersion()
        {
            try
            {
                return _systemConfigurationService.GetAppVersion();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "SystemConfiguration", ex.ToString());
                return null;
            }
        }

        public AppVersionModel Save(AppVersionInputModel requestModel)
        {
            try
            {
                return _systemConfigurationService.Save(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "SystemConfiguration", ex.ToString());
                return null;
            }
        }

        public SystemConfigurationModel GetSystemConfiguration()
        {
            try
            {
                return _systemConfigurationService.GetSystemConfiguration();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "SystemConfiguration", ex.ToString());
                return null;
            }
        }

        public SystemConfigurationModel UpdateSystemConfiguration(UpdateSystemConfigurationModel requestModel)
        {
            try
            {
                return _systemConfigurationService.UpdateSystemConfiguration(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "SystemConfiguration", ex.ToString());
                return null;
            }
        }
    }
}