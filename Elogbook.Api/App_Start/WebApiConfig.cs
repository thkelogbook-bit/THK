using Elogbook.Api.Filters;
using Microsoft.Web.Http.Routing;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Elogbook.Api
{
    /// <summary>
    /// Configure the Web API
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register Web API configuration
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            var constraintResolver = new DefaultInlineConstraintResolver() { ConstraintMap = { ["apiVersion"] = typeof(ApiVersionRouteConstraint) } };
            config.AddApiVersioning(options => options.ReportApiVersions = true);
            config.MapHttpAttributeRoutes(constraintResolver);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Add Authorize Attribute
            config.Filters.Add(new AuthorizeAttribute());
            // Fix issue validate request error is too large or small for an Int32
            // config.Filters.Add(new ValidateModelStateFilterAttribute());
            config.Filters.Add(new GlobalExceptionFilterAttribute());
        }
    }
}
