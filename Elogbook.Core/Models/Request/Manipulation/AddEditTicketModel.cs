using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class AddEditTicketModel
    {
        /// <summary>
        /// mã số phiếu thao tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

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
        /// 1.1 Chức vụ người tạo phiếu
        /// </summary>
        public string CreatedUserPositionName { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt
        /// </summary>
        public int ApprovedUserID { get; set; }

        /// <summary>
        /// 1.2 Chức vụ người phê duyệt
        /// </summary>
        public string ApprovedUserPositionName { get; set; }

        /// <summary>
        /// 1.3 Người giám sát, 1.4 Người thao tác
        /// </summary>
        public virtual List<AddEditParticipantModel> Participants { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// 5. Điều kiện cần có thể thực hiện và 6. Lưu ý
        /// </summary>
        public virtual List<AddEditOtherRequestModel> OtherRequests { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public int TicketStatusValue { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public virtual List<AddEditSequenceOperationModel> SequenceOperations { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// True: khi thực hiện chức năng Lưu thông tin, False: khi thực hiện chức năng gửi phiếu đăng ký
        /// </summary>
        public bool IsSave { get; set; }
    }
}
