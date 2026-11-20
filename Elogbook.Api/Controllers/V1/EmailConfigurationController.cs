using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models.Request.EmailNotification;
using Elogbook.Core.Models.Response.EmailNotification;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Định nghĩa những api cho phần cấu hình email hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/emailConfiguration")]
    public class EmailConfigurationController : BaseApiController
    {
        private readonly IEmailConfigurationBusiness _emailConfigurationBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailConfigurationBusiness"></param>
        /// <param name="commonBusiness"></param>
        public EmailConfigurationController(IEmailConfigurationBusiness emailConfigurationBusiness,
            ICommonBusiness commonBusiness)
        {
            _emailConfigurationBusiness = emailConfigurationBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Lấy thông tin cấu hình email hệ thống
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_CHHTEMAIL
        [Route("get")]
        [HttpGet]
        public EmailConfigurationResponseModel GetEmailConfiguration()
        {
            return _emailConfigurationBusiness.GetEmailConfiguration();
        }

        /// <summary>
        /// Lưu thông tin email hệ thống
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        public EmailConfigurationResponseModel Save([FromBody] EmailConfigurationRequestModel requestBody)
        {
            EmailConfigurationResponseModel responseModel = new EmailConfigurationResponseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CHHTEMAIL)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestBody.CurrentUserID = CurrentUser.ID;
            return _emailConfigurationBusiness.Save(requestBody);
        }
    }
}
