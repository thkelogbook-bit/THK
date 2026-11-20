using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Helpers;
using Elogbook.Core.Identity;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Elogbook.Api
{
    /// <summary>
    /// Jwt Authentication Attribute
    /// </summary>
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        /// <summary>
        /// Realm
        /// </summary>
        public string Realm { get; set; }
        /// <summary>
        /// Allow Multiple
        /// </summary>
        public bool AllowMultiple => false;

        /// <summary>
        /// Do Authenticate
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (!RuntimeManager.IsApplicationAuthorized(true, AppSettings.VALIDATION_KEY))
            {
                context.ErrorResult = new AuthenticationFailureResult(CommonConstant.INCIDENT, request);
                throw new InvalidOperationException(CommonConstant.INCIDENT);
            }

            #region PENTEST-APIKEY: ELOGBOOK_API_KEY_VERSION: RUxPR0JPT0tfQVBJX0tFWV9WRVJTSU9O

            var errorMessage = CommonConstant.API_KEY_ERROR;

            try
            {
                if (request.Headers.TryGetValues("apiKey", out IEnumerable<string> requestHeaders) && requestHeaders != null)
                {
                    var apiKey = requestHeaders.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(apiKey)
                        && apiKey.Equals(CommonConstant.API_KEY))
                    {
                        errorMessage = string.Empty;
                    }
                }
            }
            catch
            {
                errorMessage = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                context.ErrorResult = new AuthenticationFailureResult(errorMessage, request);
                return;
            }

            #endregion

            if (authorization == null || authorization.Scheme != "Bearer")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;

            #region PENTEST-CHECK TOKEN

            // Thực hiện kiểm tra nếu token của người dùng đã logout ứng dụng thì không thể truy cập cho đến khi login và sử dụng token mới
            try
            {
                using (var dbContext = new ElogbookDataContext())
                {
                    var tokenExpires = dbContext.TokenExpire.ToListAsync().Result;
                    if (tokenExpires != null
                        && tokenExpires.Exists(x => x.Token.Equals(token, StringComparison.OrdinalIgnoreCase)))
                    {
                        context.ErrorResult = new AuthenticationFailureResult("Token hết hạn sử dụng", request);
                    }
                }
            }
            catch
            {
                //ignore error
            }

            #endregion

            CustomIdentity customIdentityInfo = null;
            var principal = await AuthenticateJwtToken(token, ref customIdentityInfo);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Token không đúng", request);
            }
            else
            {
                #region PENTEST-CHECK USER

                // Kiểm tra nếu người dùng dựa trên token không tồn tại hoặc không có quyền truy cập ứng dụng thì báo lỗi
                try
                {
                    using (var dbContext = new ElogbookDataContext())
                    {
                        if (customIdentityInfo != null && customIdentityInfo.ID > 0)
                        {
                            var isExist = dbContext.Users.Any(x => x.ID == customIdentityInfo.ID && !x.IsDeleted && x.Status == (int)AccessStatus.Active);
                            if (!isExist)
                            {
                                context.ErrorResult = new AuthenticationFailureResult(CommonConstant.ACCOUNT_INVALID, request);
                            }
                        }
                        else
                        {
                            context.ErrorResult = new AuthenticationFailureResult(CommonConstant.ACCOUNT_INVALID, request);
                        }
                    }
                }
                catch
                {
                    //ignore error
                }

                #endregion

                context.Principal = principal;
            }
        }

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="customIdentity"></param>
        /// <returns></returns>
        private static bool ValidateToken(string token, out CustomIdentity customIdentity)
        {
            customIdentity = null;

            var simplePrinciple = JwtManager.GetPrincipal(token);

            if (!(simplePrinciple?.Identity is ClaimsIdentity identity))
            {
                return false;
            }

            if (!identity.IsAuthenticated)
            {
                return false;
            }

            var userData = identity.FindFirst(ClaimTypes.UserData)?.Value;
            customIdentity = XmlHelper.Deserialize<CustomIdentity>(userData);

            if (customIdentity.ID == 0)
            {
                return false;
            }

            // More validate to check whether username exists in system
            return true;
        }

        /// <summary>
        /// Authenticate the JWT
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<IPrincipal> AuthenticateJwtToken(string token, ref CustomIdentity customIdentityInfo)
        {
            if (ValidateToken(token, out var customIdentity))
            {
                customIdentityInfo = customIdentity;
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customIdentity.Username),
                    new Claim(ClaimTypes.UserData, XmlHelper.Serialize<CustomIdentity>(customIdentity))
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        /// <summary>
        /// Do Challenge
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
            {
                parameter = "realm=\"" + Realm + "\"";
            }

            context.ChallengeWith("Bearer", parameter);
        }

    }
}
