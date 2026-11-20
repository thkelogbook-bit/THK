using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api
{
    /// <summary>
    /// Authentication Failure Result
    /// </summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reasonPhrase"></param>
        /// <param name="request"></param>
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        /// <summary>
        /// Reason
        /// </summary>
        public string ReasonPhrase { get; }
        /// <summary>
        /// HTTP Request
        /// </summary>
        public HttpRequestMessage Request { get; }

        /// <summary>
        /// Execute Filter
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = Request,
                ReasonPhrase = ReasonPhrase
            };

            return response;
        }
    }
}