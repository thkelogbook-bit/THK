using Elogbook.Core.Constants;
using Elogbook.Core.Models.Request.WorkPermit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Quản lý lệnh công tác
    /// </summary>
    public class LctWorkTicketDetailModel : EntityBaseModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// lệnh đăng ký công tác
        /// </summary>
        public ValueLabelModel RegistrationTicket { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        private DateTime? _dtApprovedDate;
        /// <summary>
        /// Ngày cấp lệnh
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
        /// Ngày cấp lệnh
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Ngày cấp lệnh
        /// </summary>
        public DateTime? SignDate { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh (cấp phân xưởng)
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public ValueLabelModel WorkTicketType { get; set; }

        private int _workTicketTypeValue;
        /// <summary>
        /// Loại lệnh (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
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
        /// Tính năng lệnh
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
        public LctLeadershipUserModel AcceptorUser { get; set; }

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
        public List<LctWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người cấp lệnh
        /// </summary>
        public LctLeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// Chữ ký Người cấp lệnh
        /// </summary>
        public string ApproverSignature { get; set; }

        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public LctAllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tiếp nhận nơi làm việc
        /// </summary>
        public LctReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Kết thúc công tác, trao trả nơi làm việc
        /// </summary>
        public LctWorkFinishModel WorkFinish { get; set; }

        /// <summary>
        /// Thông tin thi hành lệnh/ công tác hầng ngày
        /// </summary>
        public LctWorkDailyInfoModel WorkDailyInfo { get; set; }

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
        public List<LctStakeholdersResponseModel> Supervisors { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public LctLeadershipUserModel LeaderUser { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public LctLeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Người xác nhận hoàn thành
        /// </summary>
        public LctLeadershipUserModel CompleteConfirmUser { get; set; }

        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        public List<LctStakeholdersResponseModel> WorkerUsers { get; set; }
/*
        /// <summary>
        /// Danh sách nhân viên đơn vị công tác và thay đổi người (nếu có)
        /// </summary>
        public List<LctLeadershipUserModel> EmployeeListByWorkUnit { get; set; }*/


        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// File lệnh đã cấp
        /// </summary>
        public int? TicketPdfID { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public List<LctWorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Thời gian Người chỉ huy ký
        /// </summary>
        public DateTime? TimeCommanderSigned { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<LctContentPositionModel> ContentPositions { get; set; }
    }
}
