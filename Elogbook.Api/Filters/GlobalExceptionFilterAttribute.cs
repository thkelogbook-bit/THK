using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Elogbook.Api.Filters
{
    /// <summary>
    /// Quản lý lỗi chung của api service
    /// </summary>
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            LogCustom.LogMessage(TrackingLogLevel.ERROR, $"Unhandled exception: {actionExecutedContext.Exception}");

            // Trả về lỗi chung, ẩn chi tiết thông tin mã nguồn trong khi trả thông báo lỗi
            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(string.Format(@"{{ ""message"": ""{0}"" }}", CommonConstant.INCIDENT)),
            };
            actionExecutedContext.Response.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        }
    }
}