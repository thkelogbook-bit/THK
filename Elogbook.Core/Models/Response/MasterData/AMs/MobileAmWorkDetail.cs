using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Thông tin chi tiết thực hiện công việc AM
    /// </summary>
    public class MobileAmWorkDetail : MobileEntityBaseModel
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
        /// Loại công việc
        /// </summary>
        public int AmWorkTypeID { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public int AmWorkID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int AmStatusID { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// Mô tả hướng dẫn
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian lưu tạm (mm:ss)
        /// </summary>
        public string StickTime { get; set; }

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
