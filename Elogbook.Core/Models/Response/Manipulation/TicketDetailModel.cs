using Elogbook.Core.Constants;
using Elogbook.Core.Models.Response.Manipulation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class TicketDetailModel : EntityBaseModel
    {
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
        /// Ngày tạo phiếu
        /// </summary>
        public string CreatedDate { get; set; }

        private DateTime _dtCreatedDate;
        /// <summary>
        /// Ngày tạo phiếu
        /// </summary>
        public DateTime DTCreatedDate
        {
            get { return _dtCreatedDate; }
            set
            {
                _dtCreatedDate = value;
                if (_dtCreatedDate != DateTime.MinValue)
                {
                    CreatedDate = _dtCreatedDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        private DateTime _dtApprovedDate;
        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime DTApprovedDate
        {
            get { return _dtApprovedDate; }
            set
            {
                _dtApprovedDate = value;
                if (_dtApprovedDate != DateTime.MinValue)
                {
                    ApprovedDate = _dtApprovedDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Đơn vị cấp phiếu (phân xưởng)
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu (công ty)
        /// </summary>
        public ValueLabelModel Company { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// 1.1 Người viết phiếu, 1.2 Người duyệt phiếu, 1.3 Người giám sát, 1.4 Người thao tác
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

        private DateTime _startTime;
        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        [JsonIgnore]
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                if (_startTime != DateTime.MinValue)
                {
                    EstimatedStartTime = _startTime.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public string EstimatedEndTime { get; set; }

        private DateTime _endTime;
        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        [JsonIgnore]
        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                if (_endTime != DateTime.MinValue)
                {
                    EstimatedEndTime = _endTime.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

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
        /// Danh sách tài liệu đính kèm
        /// </summary>
        [JsonIgnore]
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
        /// </summary>
        public List<DeliveryAccepDeviceModel> DeliveryAccepDevices { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public List<SequenceOperationDetailModel> SequenceOperations { get; set; }

        /// <summary>
        /// Danh sách sơ đồ
        /// </summary>
        public List<AttachmentModel> MapAttachments { get; set; }

        /// <summary>
        /// Danh sách sơ đồ
        /// </summary>
        public string MapAttachmentIDs { get; set; }

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
        public string WorkEndTime { get; set; }

        private DateTime? _dtWorkEndTime;
        /// <summary>
        /// Thời gian Kết thúc
        /// </summary>
        [JsonIgnore]
        public DateTime? DTWorkEndTime
        {
            get { return _dtWorkEndTime; }
            set
            {
                _dtWorkEndTime = value;
                if (_dtWorkEndTime != null && _dtWorkEndTime != DateTime.MinValue)
                {
                    WorkEndTime = _dtWorkEndTime.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public string CompletedTime { get; set; }

        private DateTime? _dtCompletedTime;
        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        [JsonIgnore]
        public DateTime? DTCompletedTime
        {
            get { return _dtCompletedTime; }
            set
            {
                _dtCompletedTime = value;
                if (_dtCompletedTime != null && _dtCompletedTime != DateTime.MinValue)
                {
                    CompletedTime = _dtCompletedTime.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// File phiếu đã cấp
        /// </summary>
        public int? TicketPdfID { get; set; }

        /// <summary>
        /// Người duyệt phiếu
        /// </summary>
        public ParticipantModel ApprovedUser { get; set; }

        /// <summary>
        /// Người tạo phiếu
        /// </summary>
        public ParticipantModel CreatorUser { get; set; }

        /// <summary>
        /// 1.3 Người giám sát
        /// </summary>
        public List<ParticipantModel> Suppervisors { get; set; }

        /// <summary>
        /// 1.3 Người thao tác
        /// </summary>
        public List<ParticipantModel> Workers { get; set; }
    }
}
