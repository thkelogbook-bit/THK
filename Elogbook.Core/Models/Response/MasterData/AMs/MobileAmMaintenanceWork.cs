using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Quản lý Công việc AM của một lịch trình
    /// </summary>
    public class MobileAmMaintenanceWork : MobileEntityBaseModel
    {

        /// <summary>
        /// Mã lịch bảo trì
        /// </summary>
        public int? AmMaintenanceScheduleID { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? AmWorkTypeID { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public int? AmWorkID { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

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
