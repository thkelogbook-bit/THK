using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Elogbook.Api
{
    /// <summary>
    /// Http Authentication Challenge
    /// </summary>
    public static class HttpAuthenticationChallengeContextExtensions
    {
        /// <summary>
        /// Challenge With
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        /// <summary>
        /// Challenge With
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="parameter"></param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        /// <summary>
        /// Challenge With
        /// </summary>
        /// <param name="context"></param>
        /// <param name="challenge"></param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }
}
