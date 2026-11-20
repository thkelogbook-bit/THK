using Elogbook.Core.Models.Response.MasterData.AMs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.SyncAMs
{
    /// <summary>
    /// Đồng bộ AM
    /// </summary>
    public class MobileAutonomousMaintenanceSyncDataModel
    {
        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách bảo trì 
        /// </summary>
        public List<MobileAutonomousMaintenanceModel> MobileAutonomousMaintenanceModels { get; set; }

        /// <summary>
        /// Danh sách chi tiết công việc của AM
        /// </summary>
        public List<AMMobileWorkDetailModel> AMMobileWorkDetailModels { get; set; }

        /// <summary>
        /// Thông tin Tạm dừng thực hiện bảo trì tự quản
        /// </summary>
        public List<AMMobileWorkPendingModel> AMMobileWorkPending { get; set; }

        /// <summary>
        /// Danh sách tiến trình thực hiện công việc
        /// </summary>
        public List<AMMobileWorkingProgress> AMMobileWorkingProgress { get; set; }

        /// <summary>
        /// Thông tin Không thực hiện bảo trì tự quản
        /// </summary>
        public List<AMMobileStopMaintenanceModel> AMMobileStopMaintenance { get; set; }
    }
}
