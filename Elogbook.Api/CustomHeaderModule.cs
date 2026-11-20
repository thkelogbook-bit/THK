using System;
using System.Web;

namespace Elogbook.Api
{
    /// <summary>
    /// Custom reponse header to remove header key Server, X-Aspnet-Version and X-Powered-By
    /// </summary>
    public class CustomHeaderModule : IHttpModule
    {
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// context
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Remove header key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Response != null && HttpContext.Current.Response.Headers != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
                HttpContext.Current.Response.Headers.Remove("X-Aspnet-Version");
                HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            }
        }
    }
}