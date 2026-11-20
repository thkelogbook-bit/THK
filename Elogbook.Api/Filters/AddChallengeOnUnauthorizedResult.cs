using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api
{
    /// <summary>
    /// Add Challenge On Unauthorized Result
    /// </summary>
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="challenge"></param>
        /// <param name="innerResult"></param>
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        /// <summary>
        /// The Challenge
        /// </summary>
        public AuthenticationHeaderValue Challenge { get; }
        /// <summary>
        /// Inner Result
        /// </summary>
        public IHttpActionResult InnerResult { get; }

        /// <summary>
        /// Execute Challenge
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);

            // Only add one challenge per authentication scheme.
            if (response.StatusCode == HttpStatusCode.Unauthorized
                && response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
            {
                try
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExecuteAsync-Validate Token:",
                        $"\nresponse:\n{CommonFunction.SerializeObjectToString(response)}");
                }
                catch (Exception)
                {
                    //ignore error
                }

                response.Headers.WwwAuthenticate.Add(Challenge);
            }

            return response;
        }
    }
}