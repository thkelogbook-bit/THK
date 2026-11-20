using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request.ActiveDirectory;
using Elogbook.Core.Models.Response.ActiveDirectory;
using System;

namespace Elogbook.Business
{
    /// <summary>
    /// Xử lý các phương thức cho cấu hình Active Directory
    /// </summary>
    public class ActiveDirectoryBusiness : IActiveDirectoryBusiness
    {
        private readonly IActiveDirectoryService _activeDirectoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activeDirectoryService"></param>
        public ActiveDirectoryBusiness(IActiveDirectoryService activeDirectoryService)
        {
            _activeDirectoryService = activeDirectoryService;
        }

        public ActiveDirectoryResponseModel GetActiveDirectoryInfo()
        {
            try
            {
                return _activeDirectoryService.GetActiveDirectoryInfo();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ActiveDirectoryBusiness.ToString(), ex.ToString());
                return null;
            }
        }

        public ActiveDirectoryResponseModel Save(ActiveDirectoryRequestModel requestModel)
        {
            try
            {
                return _activeDirectoryService.Save(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ActiveDirectoryBusiness.ToString(), ex.ToString());
                return null;
            }
        }
    }
}
