using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin bảo trì tự quản
    /// </summary>
    public class AutonomousMaintenanceModel : EntityBaseModel
    {
        /// <summary>
        /// Lịch bảo trì
        /// </summary>
        public ValueLabelCheckedModel MaintenanceSchedule { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public ValueLabelCheckedModel Location { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ValueLabelCheckedModel Device { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationPosition { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationShift { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public AmStatusInfoModel Status { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Danh sách công việc thực hiện
        /// </summary>
        public List<AmWorkDetailModel> WorkDetails { get; set; }

        /// <summary>
        /// File hình ảnh trước khi thực hiện công việc
        /// </summary>
        [JsonIgnore]
        public string BeforeImageIds { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> BeforeImages { get; set; }

        /// <summary>
        /// File hình ảnh sau khi thực hiện AM
        /// </summary>
        [JsonIgnore]
        public string AfterImageIds { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> AfterImages { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public ValueLabelCheckedModel DeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Mô tả bất thường thiết bị
        /// </summary>
        public string DeviceAbnormalDescription { get; set; }

        /// <summary>
        /// Các file bất thường thiết bị, Hình ảnh or Video
        /// </summary>
        [JsonIgnore]
        public string DeviceAbnormalAttachmentIds { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> DeviceAbnormalAttachments { get; set; }

        /// <summary>
        /// Lý do không thực hiện bảo trì
        /// </summary>
        public AmStopMaintenanceModel StopMaintenance { get; set; }

        /// <summary>
        /// ID phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Người hoàn thành
        /// </summary>
        public ValueLabelCheckedModel CompleterUser { get; set; }

        /// <summary>
        /// UserId người hoàn thành
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
        /// Tự động hoàn thành
        /// </summary>
        public bool IsAutoComplete { get; set; } = false;

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
        /// ID Ca vận hành
        /// </summary>
        [JsonIgnore]
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Tên Ca vận hành
        /// </summary>
        [JsonIgnore]
        public string OperationShiftName { get; set; }

        /// <summary>
        /// ID Thiết bị
        /// </summary>
        [JsonIgnore]
        public int DeviceID { get; set; }

        /// <summary>
        /// Tên Thiết bị
        /// </summary>
        [JsonIgnore]
        public string DeviceName { get; set; }

        /// <summary>
        /// Tiến trình thực hiện bảo trì tự quản
        /// </summary>
        public List<AmWorkingProgressModel> AmWorkingProgress { get; set; }

        /// <summary>
        /// Mã phiếu yêu cầu
        /// </summary>
        public int? AmRequestTicketID { get; set; }
    }
}
