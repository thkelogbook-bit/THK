using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.ActiveDirectory
{
    /// <summary>
    /// Thông tin Active Directory
    /// </summary>
    public class ActiveDirectoryRequestModel
    {
        /// <summary>
        /// Cho phép sử dụng AD
        /// </summary>
        public bool IsEnableActiveDirectory { get; set; }

        /// <summary>
        /// IP Address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Domain Controller
        /// </summary>
        public string DomainController { get; set; }

        /// <summary>
        /// Organizational Unit
        /// </summary>
        public string OrganizationalUnit { get; set; }

        /// <summary>
        /// Người dùng login ứng dụng
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
