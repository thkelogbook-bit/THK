using System;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Bảo trì tự quản - AutonomousMaintenance
    /// </summary>
    public class MobileAutonomousMaintenance : MobileEntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int AmLocationID { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int AmDeviceID { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Lịch thực hiện
        /// </summary>
        public int AmMaintenanceScheduleID { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int AmStatusID { get; set; }

        /// <summary>
        /// Có bất thường
        /// </summary>
        public bool? IsAbnormal { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public int? AmDeviceAbnormalStatusID { get; set; }

        /// <summary>
        /// File hình ảnh trước khi thực hiện AM
        /// </summary>
        public string BeforeImages { get; set; }

        /// <summary>
        /// File hình ảnh sau khi thực hiện AM
        /// </summary>
        public string AfterImages { get; set; }

        /// <summary>
        /// Mô tả bất thường thiết bị
        /// </summary>
        public string DeviceAbnormalDescription { get; set; }

        /// <summary>
        /// Các file bất thường thiết bị, Hình ảnh or Video
        /// </summary>
        public string DeviceAbnormalAttachments { get; set; }

        /// <summary>
        /// ID phiếu yêu cầu
        /// </summary>
        public int? AmRequestTicketID { get; set; }

        /// <summary>
        /// Người hoành thành
        /// </summary>
        public int? CompleterUserID { get; set; }

        /// <summary>
        /// Chữ ký Người hoành thành
        /// </summary>
        public string CompleterSignatureContent { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? CompleteDate { get; set; }

        /// <summary>
        /// Đã gửi mail nhắc lịch 
        /// </summary>
        public bool IsSendNotify { get; set; }

        /// <summary>
        /// Tự động hoàn thành
        /// </summary>
        public bool IsAutoComplete { get; set; }

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

        /// <summary>
        /// Đánh dấu việc đồng bộ dữ liệu AM sang NKVH
        /// </summary>
        public bool IsSyncToOperationDiary { get; set; }
    }
}
