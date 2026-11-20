using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// ManipulationTicketDetailModel
    /// </summary>
    public class ManipulationTicketDetailModel
    {
        /// <summary>
        /// Mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        public int TicketTypeValue { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public virtual ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public virtual ValueLabelModel ApproveUnit { get; set; }

        /// <summary>
        /// Tên hiển thị đơn vị cấp phiếu
        /// </summary>
        public string ApproveUnitView { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// 1.1 Chức vụ người tạo phiếu
        /// </summary>
        public string CreatedUserPositionName { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt
        /// </summary>
        public virtual ValueLabelModel ApprovedUser { get; set; }

        /// <summary>
        /// 1.2 Chức vụ người phê duyệt
        /// </summary>
        public string ApprovedUserPositionName { get; set; }

        /// <summary>
        /// 1.3 Người giám sát, 1.4 Người thao tác
        /// </summary>
        public virtual List<ParticipantModel> Participants { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// 5. Điều kiện cần có thể thực hiện và 6. Lưu ý
        /// </summary>
        public virtual List<OtherRequestModel> OtherRequests { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<AttachmentModel> FileAttachments { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public virtual ValueLabelModel TicketStatus { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public virtual List<SequenceOperationModel> SequenceOperations { get; set; }

        /// <summary>
        /// Thông tin trạng thái phiếu thao tác
        /// </summary>
        public virtual List<TicketStatusHistoryModel> TicketStatusHistories { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public virtual ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public virtual ValueLabelModel LastUpdatedUser { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }
    }
}
