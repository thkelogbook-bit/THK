using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Quản lý lịch thực hiện AM
    /// </summary>
    public class MobileAmMaintenanceSchedule : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên lịch trình
        /// </summary>
        public string MaintenanceScheduleName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }
        
        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int? AmLocationID { get; set; }
        
        /// <summary>
        /// Thiết bị
        /// </summary>
        public int? AmDeviceID { get; set; }
        
        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int? OperationPositionID { get; set; }
       
        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public int? OperationShiftID { get; set; }

        /// <summary>
        /// CronJobFormat
        /// </summary>
        public string CronJobFormat { get; set; }

        /// <summary>
        /// CronJobDesc
        /// </summary>
        public string CronJobDesc { get; set; }

        /// <summary>
        /// NotificationsTime
        /// </summary>
        public int? NotificationsTime { get; set; }

        /// <summary>
        /// Trạng thái 1: Có thể sử dụng, 2: Không thể sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Ngày bắt đầu của 1 lịch trình
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public DateTime EndDate { get; set; }

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
