using Elogbook.Core.Models.Request.ActiveDirectory;
using Elogbook.Core.Models.Response.ActiveDirectory;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức sử dụng cho cấu hình Active Directory
    /// </summary>
    public interface IActiveDirectoryService
    {
        /// <summary>
        /// Lấy thông tin cấu hình Active Directory
        /// </summary>
        /// <returns></returns>
        ActiveDirectoryResponseModel GetActiveDirectoryInfo();

        /// <summary>
        /// Lưu thông tin Active Directory
        /// </summary>
        /// <returns></returns>
        ActiveDirectoryResponseModel Save(ActiveDirectoryRequestModel requestModel);
    }
}
