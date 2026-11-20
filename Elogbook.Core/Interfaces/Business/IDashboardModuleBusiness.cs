using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Business layer cho danh mục module dashboard
    /// </summary>
    public interface IDashboardModuleBusiness
    {
        /// <summary>
        /// Lấy danh sách module đang active
        /// </summary>
        ResponseModel<List<DashboardModuleModel>> GetActiveModules();
    }
}
