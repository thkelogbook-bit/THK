using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request.EmailNotification;
using Elogbook.Core.Models.Response.EmailNotification;
using System;

namespace Elogbook.Business
{
    /// <summary>
    /// Xử lý các phương thức cho cấu hình hệ thống
    /// </summary>
    public class EmailConfigurationBusiness : IEmailConfigurationBusiness
    {
        private readonly IEmailConfigurationService _emailConfigurationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailConfigurationService"></param>
        public EmailConfigurationBusiness(IEmailConfigurationService emailConfigurationService)
        {
            _emailConfigurationService = emailConfigurationService;
        }

        public EmailConfigurationResponseModel GetEmailConfiguration()
        {
            try
            {
                return _emailConfigurationService.GetEmailConfiguration();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "EmailConfiguration", ex.ToString());
                return null;
            }
        }

        public EmailConfigurationResponseModel Save(EmailConfigurationRequestModel requestModel)
        {
            try
            {
                return _emailConfigurationService.Save(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "EmailConfiguration", ex.ToString());
                return null;
            }
        }
    }
}
