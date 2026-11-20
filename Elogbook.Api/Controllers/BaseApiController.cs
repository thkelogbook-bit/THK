using Elogbook.Core.Helpers;
using Elogbook.Core.Identity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;

namespace Elogbook.Api.Controllers
{
    /// <summary>
    /// The base api controller
    /// </summary>
    [JwtAuthentication]
    public class BaseApiController : ApiController
    {
        private CustomIdentity _currentUser;

        /// <summary>
        /// Get the current identity
        /// </summary>
        protected internal virtual CustomIdentity CurrentUser
        {
            get
            {
                if (_currentUser == null && User.Identity.IsAuthenticated)
                {
                    var userData = (User as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);

                    if (userData != null)
                    {
                        _currentUser = XmlHelper.Deserialize<CustomIdentity>(userData.Value);
                    }
                }

                return _currentUser;
            }
        }

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected HttpResponseMessage Download(HttpContent content, string fileName = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected HttpResponseMessage Download(Stream stream, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                var fs = stream;
                fs.Position = 0;
                response.Content = new StreamContent(fs);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}
