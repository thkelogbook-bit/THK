namespace Elogbook.Core.Models.Response.ActiveDirectory
{
    /// <summary>
    /// Thông tin Active Directory
    /// </summary>
    public class ActiveDirectoryResponseModel : EntityBaseModel
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
        /// StatusCode
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
