using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Quản lý phiếu công tác
    /// </summary>
    public class WorkTicketDetailModel : EntityBaseModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public ValueLabelModel RegistrationTicket { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        private DateTime? _dtApprovedDate;
        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        [JsonIgnore]
        public DateTime? DTDApprovedDate
        {
            get { return _dtApprovedDate; }
            set
            {
                _dtApprovedDate = value;
                if (_dtApprovedDate != null && _dtApprovedDate != DateTime.MinValue)
                {
                    ApprovedDate = _dtApprovedDate.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                }
            }
        }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime? SignDate { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu (cấp phân xưởng)
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public ValueLabelModel WorkTicketType { get; set; }

        private int _workTicketTypeValue;
        /// <summary>
        /// Loại phiếu (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        [JsonIgnore]
        public int WorkTicketTypeValue
        {
            get
            {
                return _workTicketTypeValue;
            }
            set
            {
                _workTicketTypeValue = value;
                switch (value)
                {
                    case 1:
                        WorkTicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_INSIDEPLAN };
                        break;
                    case 2:
                        WorkTicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_OUTSIDEPLAN };
                        break;
                    case 3:
                        WorkTicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_SPECIALCASE };
                        break;
                    default:
                        WorkTicketType = new ValueLabelModel();
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel WorkTicketStatus { get; set; }

        /// <summary>
        /// Tính năng phiếu
        /// </summary>
        public ValueLabelModel WorkTicketFeature { get; set; }

        /// <summary>
        /// Nhân viên đơn vị công tác
        /// </summary>
        public int? UnitNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LeadershipUserModel AcceptorUser { get; set; }

        /// <summary>
        /// Đại diện đơn vị quản lý
        /// </summary>
        public LeadershipUserModel RepresentativeUnitUser { get; set; }

        /// <summary>
        /// Phạm vi công tác
        /// </summary>
        public string WorkScope { get; set; }

        private DateTime _dtStartDate;
        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        [JsonIgnore]
        public DateTime DTStartDate
        {
            get { return _dtStartDate; }
            set
            {
                _dtStartDate = value;
                if (_dtStartDate != DateTime.MinValue)
                {
                    StartDate = _dtStartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime EstimatedStartTime { get; set; }

        private DateTime _dtEndDate;
        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        [JsonIgnore]
        public DateTime DTEndDate
        {
            get { return _dtEndDate; }
            set
            {
                _dtEndDate = value;
                if (_dtEndDate != DateTime.MinValue)
                {
                    EndDate = _dtEndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime EstimatedEndTime { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Danh sách mối nguy hiểm
        /// </summary>
        public List<WarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public LeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// Chữ ký Người cấp phiếu
        /// </summary>
        public string ApproverSignature { get; set; }

        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public AllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tiếp nhận nơi làm việc
        /// </summary>
        public ReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Kết thúc công tác, trao trả nơi làm việc
        /// </summary>
        public WorkFinishModel WorkFinish { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<AttachmentModel> Attachments { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string AttachmentsID { get; set; }

        /// <summary>
        /// Kết quả công tác
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> WorkPictures { get; set; }

        /// <summary>
        /// Hình ảnh đính kèm
        /// </summary>
        public string WorkPicturesID { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public List<StakeholdersResponseModel> Supervisors { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public LeadershipUserModel LeaderUser { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public LeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp ký cuối cùng
        /// </summary>
        public LeadershipUserModel CommanderLastUser { get; set; }

        /// <summary>
        /// Người xác nhận hoàn thành
        /// </summary>
        public LeadershipUserModel CompleteConfirmUser { get; set; }

        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        public List<StakeholdersResponseModel> WorkerUsers { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// File phiếu đã cấp
        /// </summary>
        public int? TicketPdfID { get; set; }

        /// <summary>
        /// Loại Phiếu
        /// </summary>
        public List<WorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Thay đổi người cho phép
        /// </summary>
        public List<WorkTicketTrackingModel> WorkTicketTrackings { get; set; }

        /// <summary>
        /// Thông tin phiếu đăng ký
        /// </summary>
        public BasicDetailModel RegistTicketInfo { get; set; }

        /// <summary>
        /// Thông tin đợt sửa chữa lớn
        /// </summary>
        public BasicDetailModel MajorRepairInfo { get; set; }
    }
}
