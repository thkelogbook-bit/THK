using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.Dashboard;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Quản lý danh mục module dashboard
    /// </summary>
    public class DashboardModuleService : IDashboardModuleService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        public DashboardModuleService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách module đang active, sắp xếp theo Order
        /// </summary>
        public List<DashboardModuleModel> GetActiveModules()
        {
            return _unitOfWork.ElogbookRepository<DashboardModule>()
                .GetQuery(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .Select(x => new DashboardModuleModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    ModuleValue = x.ModuleValue,
                    Order = x.Order,
                    Description = x.Description,
                })
                .ToList();
        }
    }
}
