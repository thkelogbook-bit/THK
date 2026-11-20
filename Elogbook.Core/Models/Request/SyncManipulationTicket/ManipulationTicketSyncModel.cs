using Elogbook.Core.Models.Request.Manipulation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin PTT
    /// </summary>
    public class ManipulationTicketSyncModel
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// mã số đăng ký phiếu thao tác
        /// </summary>
        public int RegistrationManipulationTicketID { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách giao nhận nghiệm thu đường dây,  thiết bị điện trước khi thao tác
        /// </summary>
        public List<DeliveryAccepDeviceSyncModel> DeliveryAccepDeviceModels { get; set; }

        /// <summary>
        /// Thông tin hủy phiếu
        /// </summary>
        public List<CancelTicketSyncModel> CancelTicketSyncModels { get; set; }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public int? ManipulationTicketID { get; set; }

        /// <summary>
        /// Tạo mới
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Loại phiếu 1/kế hoạch 2/Đột xuất
        /// </summary>
        public ValueLabelModel TicketType { get; set; }
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Ngày viết phiếu đăng ký
        /// </summary>
        public DateTime CreatedDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public int? ApproveUnitID { get; set; }

        /// <summary>
        /// Tên hiển thị đơn vị cấp phiếu
        /// </summary>
        public string ApproveUnitView { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// 1.1 Tên người viết phiếu
        /// </summary>
        public string CreatedUserTicket { get; set; }

        /// <summary>
        /// 1.2 Tên người duyệt phiếu
        /// </summary>
        public string ApprovedUserTicket { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public DateTime StartTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public DateTime EndTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// Các sự kiện bất thường trong thao tác
        /// </summary>
        public string CaseAbnormal { get; set; }

        /// <summary>
        /// Thời gian Kết thúc
        /// </summary>
        public DateTime? WorkEndTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Danh sách sơ đồ
        /// </summary>
        public string MapAttachmentIDs { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public int TicketStatusValue { get; set; }
        /// <summary>
        /// người giám sát và người thao tác
        /// </summary>
        public List<UserInfoInTicketModel> Participants { get; set; }
        /// <summary>
        /// Điều kiện cần có thể thực hiện và 6.Lưu ý
        /// </summary>
        public List<OtherRequestSyncModel> OtherRequests { get; set; }
        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public List<SequenceOperationSyncModel> SequenceOperations { get; set; }

        /// <summary>
        /// Người dùng ký
        /// </summary>
        public List<UserInfoInTicketModel> Suppervisors { get; set; }

        /// <summary>
        /// Người thao tác 1
        /// </summary>
        public List<UserInfoInTicketModel> Workers { get; set; }


    }
}
