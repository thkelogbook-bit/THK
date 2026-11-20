using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Tạm dừng hoặc Dừng thực hiện công việc
    /// </summary>
    public class MobileAmWorkPending : MobileEntityBaseModel
    {
        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Mã công việc
        /// </summary>
        public int AmWorkDetailID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int AmLocationID { get; set; }

        /// <summary>
        /// Lý do tạm dừng công việc
        /// </summary>
        public int? AmReasonForPendingID { get; set; }

        /// <summary>
        /// Lý do dừng công việc
        /// </summary>
        public int? AmReasonForStopID { get; set; }

        /// <summary>
        /// Người tạm dừng thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng
        /// </summary>
        public DateTime PendingTime { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; }
    }
}
