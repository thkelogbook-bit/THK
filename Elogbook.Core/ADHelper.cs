using Elogbook.Core.Constants;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using System;
using System.DirectoryServices;

namespace Elogbook.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class ADHelper
    {
        /// <summary>
        /// Hàm này kết nối đến AD server để tìm kiếm và lấy thông tin người dùng
        /// </summary>
        public static ADReponse GetActiveDirectoryUser(string userName)
        {
            ADReponse aDReponse = new ADReponse();

            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry(AppSettings.AD_LDAP))
                {
                    directoryEntry.Username = AppSettings.AD_USERNAME;
                    directoryEntry.Password = AppSettings.AD_PASSWORD;
                    directoryEntry.AuthenticationType = AuthenticationTypes.Secure;

                    using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
                    {
                        // Set the filter
                        directorySearcher.Filter = string.Format("(&(sAMAccountName={0})(objectCategory=person)(objectClass=user))", userName);

                        // Set the properties to load.
                        directorySearcher.PropertiesToLoad.Add("givenName"); // FirstName
                        directorySearcher.PropertiesToLoad.Add("sn"); // LastName
                        directorySearcher.PropertiesToLoad.Add("sAMAccountName"); // UserName

                        var user = directorySearcher.FindOne();
                        if (user != null)
                        {
                            ADUserInfo userInfo = new ADUserInfo();

                            if (user.Properties["sn"].Count > 0)
                            {
                                userInfo.FullName = user.Properties["sn"][0].ToString().Trim();
                            }

                            if (user.Properties["givenName"].Count > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(userInfo.FullName))
                                {
                                    userInfo.FullName = string.Format("{0} {1}", userInfo.FullName.Trim(), user.Properties["givenName"][0].ToString().Trim());
                                }
                                else
                                {
                                    userInfo.FullName = user.Properties["givenName"][0].ToString().Trim();
                                }
                            }

                            if (user.Properties["sAMAccountName"].Count > 0)
                            {
                                userInfo.UserName = user.Properties["sAMAccountName"][0].ToString().Trim();
                            }

                            aDReponse.UserInfo = userInfo;
                        }
                        else
                        {
                            aDReponse.ErrorCode = StatusCodeCustom.ADUserNotFound;
                            aDReponse.ErrorMessage = CommonConstant.ACCOUNT_NOTFOUND;
                        }
                    }
                }

                return aDReponse;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(Enums.TrackingLogLevel.ERROR, ex.ToString());
                aDReponse.ErrorCode = StatusCodeCustom.ADCommonError;
                aDReponse.ErrorMessage = CommonConstant.AD_COMMON_ERROR;

                return aDReponse;
            }
        }

        /// <summary>
        /// Xác thực người dùng trên AD
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ADUserInfo ValidateADUser(string userName, string password)
        {
            ADUserInfo userInfo = new ADUserInfo();

            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry(AppSettings.AD_LDAP))
                {
                    directoryEntry.Username = userName.Trim();
                    directoryEntry.Password = password.Trim();
                    directoryEntry.AuthenticationType = AuthenticationTypes.Secure;

                    using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
                    {
                        // Set the filter
                        directorySearcher.Filter = string.Format("(&(sAMAccountName={0})(objectCategory=person)(objectClass=user))", userName);

                        // Set the properties to load.
                        directorySearcher.PropertiesToLoad.Add("givenName"); // FirstName
                        directorySearcher.PropertiesToLoad.Add("sn"); // LastName
                        directorySearcher.PropertiesToLoad.Add("sAMAccountName"); // UserName

                        var user = directorySearcher.FindOne();
                        if (user != null)
                        {
                            if (user.Properties["sn"].Count > 0)
                            {
                                userInfo.FullName = user.Properties["sn"][0].ToString().Trim();
                            }

                            if (user.Properties["givenName"].Count > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(userInfo.FullName))
                                {
                                    userInfo.FullName = string.Format("{0} {1}", userInfo.FullName.Trim(), user.Properties["givenName"][0].ToString().Trim());
                                }
                                else
                                {
                                    userInfo.FullName = user.Properties["givenName"][0].ToString().Trim();
                                }
                            }

                            if (user.Properties["sAMAccountName"].Count > 0)
                            {
                                userInfo.UserName = user.Properties["sAMAccountName"][0].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(Enums.TrackingLogLevel.ERROR, ex.ToString());
                userInfo = null;
            }

            return userInfo;
        }
    }
}
