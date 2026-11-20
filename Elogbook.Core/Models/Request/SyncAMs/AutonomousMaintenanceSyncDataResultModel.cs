using Elogbook.Core.Models.Response.MasterData.AMs;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.SyncAMs
{
    /// <summary>
    /// Thông tin dữ liệu AM trả về Mobile
    /// </summary>
    public class AutonomousMaintenanceSyncDataResultModel
    {
        /// <summary>
        /// Dữ liệu thực hiện công việc AM
        /// </summary>
        public List<MobileAutonomousMaintenance> MobileAutonomousMaintenances { get; set; }

        /// <summary>
        /// Chi tiết thực hiện AM
        /// </summary>
        public List<MobileAmWorkDetail> MobileAmWorkDetails { get; set; }

        /// <summary>
        /// Dừng thực hiện công việc
        /// </summary>
        public List<MobileAmStopMaintenance> MobileAmStopMaintenances { get; set; }

        /// <summary>
        /// Tạm dừng thực hiện công việc
        /// </summary>
        public List<MobileAmWorkPending> MobileAmWorkPendings { get; set; }

        /// <summary>
        /// Tiến trình xử lý công việc
        /// </summary>
        public List<MobileAmWorkingProgress> MobileAmWorkingProgresss { get; set; }
    }
}
