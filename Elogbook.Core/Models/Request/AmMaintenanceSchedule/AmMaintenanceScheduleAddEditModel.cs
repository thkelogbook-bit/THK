using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số thêm mới/ cập nhật Lịch bảo trì
    /// </summary>
    public class AmMaintenanceScheduleAddEditModel
    {
        /// <summary>
        /// Mã lịch trình
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên lịch trình
        /// </summary>
        public string MaintenanceScheduleName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int SiteDescID { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int AssetDescID { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Danh sách công việc AM được thực hiện
        /// </summary>
        public List<AmMaintenanceWorkAddEditModel> MaintenanceWorks { get; set; }

        /// <summary>
        /// Thời gian bắt đầu run
        /// </summary>
        public string CronJobFormat { get; set; }

        /// <summary>
        /// Mô tả cron jon
        /// </summary>
        public string CronJobDesc { get; set; }

        /// <summary>
        /// Thời gian thông báo (số phút)
        /// </summary>
        public int? NotificationsTime { get; set; }

        /// <summary>
        /// Ngày bắt đầu của 1 lịch trình
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
