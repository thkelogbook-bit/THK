using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phiếu thao tác
    /// </summary>
    public class MobileManipulationTicketInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Loại phiếu: 1/kế hoạch 2/Đột xuất
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationTicketID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Ngày viết phiếu đăng ký
        /// </summary>
        public DateTime CreatedDate { get; set; }

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
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

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
    }
}
