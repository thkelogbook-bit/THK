using Elogbook.Core.Models.Response;
using System;

namespace Elogbook.Core.Models.Request.SyncAMs
{
    /// <summary>
    /// Đồng bộ lý do dừng công việc
    /// </summary>
    public class AMMobileStopMaintenanceModel : MobileEntityBaseModel
    {
        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int AmLocationID { get; set; }

        /// <summary>
        /// Lý do dừng bảo trì
        /// </summary>
        public int? AmReasonForStopID { get; set; }

        /// <summary>
        /// Người dừng thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Chữ ký Người dừng thực hiện
        /// </summary>
        public string ImplementerSignatureContent { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian dừng
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }
    }
}
