using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Elogbook.Api.Filters
{
    /// <summary>
    /// Quản lý lỗi chung của api service
    /// </summary>
    public class ValidateModelStateFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Xử lý và log thông tin lỗi
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                actionContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e =>
                        $"{x.Key}: {GetErrorMessage(e)}"))
                );

                var request = actionContext.Request;
                var descriptor = actionContext.ActionDescriptor;
                var user = actionContext.RequestContext?.Principal?.Identity?.Name ?? "anonymous";

                var logMessage =
                    $"Model validation failed\n" +
                    $"Method: {request.Method} {request.RequestUri?.PathAndQuery}\n" +
                    $"Action: {descriptor.ControllerDescriptor.ControllerName}.{descriptor.ActionName}\n" +
                    $"User: {user}\n" +
                    $"Errors: {errors}";

                LogCustom.LogMessage(TrackingLogLevel.WARN, $"Model validation failed: {logMessage}");

                // Trả về lỗi chung, ẩn chi tiết thông tin mã nguồn trong khi trả thông báo lỗi
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { message = CommonConstant.INCIDENT }
                );
            }
        }

        private static string GetErrorMessage(System.Web.Http.ModelBinding.ModelError error)
        {
            if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                return error.ErrorMessage;

            // Lỗi deserialize JSON nằm ở Exception, không nằm ở ErrorMessage
            return error.Exception?.GetBaseException().Message ?? "(no message)";
        }
    }
}