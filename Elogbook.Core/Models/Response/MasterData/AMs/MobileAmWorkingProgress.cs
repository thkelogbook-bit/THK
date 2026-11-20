using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Tiến trình thực hiện AM
    /// </summary>
    public class MobileAmWorkingProgress : MobileEntityBaseModel
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Mã chi tiết thực hiện công việc AM
        /// </summary>
        public int? AmWorkDetailID { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? AmWorkTypeID { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public int? AmWorkID { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public int? ReasonID { get; set; }

        /// <summary>
        /// ReasonType : 1:Pending, 2:Stop
        /// </summary>
        public int? ReasonType { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Remark { get; set; }

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
