using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.SyncAMs;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmMobileDataSyncBusiness
    {
        /// <summary>
        /// Đồng bộ AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SyncAM(MobileAutonomousMaintenanceSyncDataModel requestModel);
    }
}
