using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response.Manipulation
{
		/// <summary>
		/// Thông tin chi tiết phiếu thao tác 
		/// </summary>
		public class TicketDetailWebModel
		{
        /// <summary>
        /// Mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Loại phiếu
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime ApprovedDate { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// 1.1 Người viết phiếu
        /// </summary>
        public ParticipantModel CreatedUser { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt
        /// </summary>
        public ParticipantModel ApprovedUser { get; set; }

        /// <summary>
        /// 1.3 Người giám sát, 1.4 Người thao tác
        /// </summary>
        public List<ParticipantModel> Participants { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public string EstimatedStartTime { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public string EstimatedEndTime { get; set; }

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// 5. Điều kiện cần có thể thực hiện và 6. Lưu ý
        /// </summary>
        public List<OtherRequestModel> OtherRequests { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public List<AttachmentModel> ReferAttachments { get; set; }

        /// <summary>
        /// Giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
        /// </summary>
        public List<DeliveryAccepDeviceModel> DeliveryAccepDevices { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public List<SequenceOperationModel> SequenceOperations { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public List<AttachmentModel> MapAttachments { get; set; }

        /// <summary>
        /// Thông tin hủy phiếu
        /// </summary>
        public CancelTicketInfoModel CancelTicketInfo { get; set; }

        /// <summary>
        /// Các sự kiện bất thường trong thao tác
        /// </summary>
        public string CaseAbnormal { get; set; }

        /// <summary>
        /// Thời gian Kết thúc
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public string CompletedTime { get; set; }
    }
}
