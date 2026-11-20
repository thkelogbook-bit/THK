using Elogbook.Api.Controllers;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Elogbook.Api
{
    /// <summary>
    /// Define methods để kiểm tra quyền và thông báo lỗi
    /// Hỗ trợ check nhiều quyền với logic OR (user cần có ít nhất một quyền)
    /// </summary>
    public class PermissionAttribute : AuthorizeAttribute
    {
        private readonly EnumUserPermission[] _permissions;

        /// <summary>
        /// Constructor - nhận một hoặc nhiều quyền
        /// </summary>
        /// <param name="permissions">Danh sách quyền cần check (logic OR)</param>
        public PermissionAttribute(params EnumUserPermission[] permissions)
        {
            _permissions = permissions ?? new EnumUserPermission[0];
        }

        /// <summary>
        /// Kiểm tra quyền của người dùng
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!actionContext.RequestContext
                .Principal
                .Identity
                .IsAuthenticated)
            {
                return false;
            }

            var controller = actionContext.ControllerContext.Controller as BaseApiController;

            if ((controller?.CurrentUser) != null)
            {
                var commonBusiness = actionContext.Request
                    .GetDependencyScope()
                    .GetService(typeof(ICommonBusiness)) as ICommonBusiness;

                foreach (var permission in _permissions)
                {
                    if (commonBusiness.CheckUserHasRight(controller.CurrentUser.ID, permission.ToString()).Data)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Custom để trả về thông báo lỗi rõ ràng
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response =
                actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new ResponseModel<bool>
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Data = false,
                        ErrorMessages =
                        {
                            CommonConstant.USER_RIGHT
                        }
                    });
        }

    }
}