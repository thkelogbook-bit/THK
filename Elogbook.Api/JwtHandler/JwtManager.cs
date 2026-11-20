using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Helpers;
using Elogbook.Core.Identity;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Elogbook.Api
{
    /// <summary>
    /// Jwt Manager
    /// </summary>
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        /// <summary>
        ///  Generate token
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="tokenExpireMinutes"></param>
        /// <returns></returns>
        public static TokenResponseModel GenerateToken(CustomIdentity identity, int tokenExpireMinutes = 0)
        {
            var expireMinutes = tokenExpireMinutes;
            // If not input toke expire then get default token expire in web config setting
            if (tokenExpireMinutes <= 0)
                expireMinutes = AppSettings.DEFAULT_TOKEN_EXPIRE_MINUTES;

            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, identity.Username),
                            new Claim(ClaimTypes.UserData, XmlHelper.Serialize<CustomIdentity>(identity)),
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new TokenResponseModel { AccessToken = token, TokenExpireTime = expireMinutes };
        }

        /// <summary>
        /// Get principal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "GetPrincipal",
                        $"\ntoken:\n{ex}");
                return null;
            }
        }
    }
}