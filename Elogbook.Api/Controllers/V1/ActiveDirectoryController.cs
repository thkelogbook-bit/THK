using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models.Request.ActiveDirectory;
using Elogbook.Core.Models.Response.ActiveDirectory;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Định nghĩa những api cho phần cấu hình Active Directory
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/activeDirectory")]
    public class ActiveDirectoryController : BaseApiController
    {
        private readonly IActiveDirectoryBusiness _activeDirectoryBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activeDirectoryBusiness"></param>
        public ActiveDirectoryController(IActiveDirectoryBusiness activeDirectoryBusiness)
        {
            _activeDirectoryBusiness = activeDirectoryBusiness;
        }

        /// <summary>
        /// Lấy thông tin cấu hình Active Directory
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_CHHT
        [Route("get")]
        [HttpGet]
        public ActiveDirectoryResponseModel GetActiveDirectory()
        {
            return _activeDirectoryBusiness.GetActiveDirectoryInfo();
        }

        /// <summary>
        /// Lưu thông tin Active Directory
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        public ActiveDirectoryResponseModel Save([FromBody] ActiveDirectoryRequestModel requestBody)
        {
            requestBody.CurrentUserID = CurrentUser.ID;
            return _activeDirectoryBusiness.Save(requestBody);
        }
    }
}
