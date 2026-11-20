using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.SyncAMs;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Phiếu thao tác
    /// </summary>
    public interface IAmMobileSyncDataService
    {
        /// <summary>
        /// Đồng bộ AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SyncAM(MobileAutonomousMaintenanceSyncDataModel requestModel);

    }
}
