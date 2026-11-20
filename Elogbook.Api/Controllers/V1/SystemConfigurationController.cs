using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models.Request.SystemConfiguration;
using Elogbook.Core.Models.Response.SystemConfiguration;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Định nghĩa những api cho phần cấu hình hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/systemConfiguration")]
    public class SystemConfigurationController : BaseApiController
    {
        private readonly ISystemConfigurationBusiness _systemConfigurationBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemConfigurationBusiness"></param>
        /// <param name="commonBusiness"></param>
        public SystemConfigurationController(ISystemConfigurationBusiness systemConfigurationBusiness,
            ICommonBusiness commonBusiness)
        {
            _systemConfigurationBusiness = systemConfigurationBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Lấy version mobile app
        /// </summary>
        /// <returns></returns>
        [Route("getAppVersion")]
        [HttpGet]
        [AllowAnonymous]
        public AppVersionModel GetAppVersion()
        {
            return _systemConfigurationBusiness.GetAppVersion();
        }

        /// <summary>
        /// Cập nhật version
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        public AppVersionModel Save([FromBody] AppVersionInputModel requestBody)
        {
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CHHT)).Data)
            {
                return null;
            }

            return _systemConfigurationBusiness.Save(requestBody);
        }

        /// <summary>
        /// Lấy cấu hình hệ thống
        /// </summary>
        /// <returns></returns>
        [Route("getSystemConfiguration")]
        [HttpGet]
        public SystemConfigurationModel GetSystemConfiguration()
        {
            return _systemConfigurationBusiness.GetSystemConfiguration();
        }

        /// <summary>
        /// Cập nhật cấu hình hệ thống
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("updateSystemConfiguration")]
        [HttpPut]
        public SystemConfigurationModel UpdateSystemConfiguration([FromBody] UpdateSystemConfigurationModel requestBody)
        {
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CHHT)).Data)
            {
                return null;
            }

            return _systemConfigurationBusiness.UpdateSystemConfiguration(requestBody);
        }
    }
}
