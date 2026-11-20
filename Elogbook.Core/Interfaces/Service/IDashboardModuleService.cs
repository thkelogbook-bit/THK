using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý danh mục module dashboard
    /// </summary>
    public interface IDashboardModuleService
    {
        /// <summary>
        /// Lấy danh sách module đang active, sắp xếp theo Order
        /// </summary>
        List<DashboardModuleModel> GetActiveModules();
    }
}
