using Elogbook.Core.Constants;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Đăng ký phiếu công tác
    /// </summary>
    public class RegistrationTicketModel : EntityBaseModel
    {
        /// <summary>
        /// Loại phiếu (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        private int _ticketTypeValue;
        /// <summary>
        /// Loại phiếu (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        [JsonIgnore]
        public int TicketTypeValue
        {
            get
            {
                return _ticketTypeValue;
            }
            set
            {
                _ticketTypeValue = value;
                switch (value)
                {
                    case 1:
                        TicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_INSIDEPLAN };
                        break;
                    case 2:
                        TicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_OUTSIDEPLAN };
                        break;
                    case 3:
                        TicketType = new ValueLabelModel { Value = value, Label = CommonConstant.TICKET_SPECIALCASE };
                        break;
                    default:
                        TicketType = new ValueLabelModel();
                        break;
                }
            }
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Số phiếu đăng ký công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Đơn vị nhận phiếu
        /// </summary>
        public ValueLabelModel ReceiveUnit { get; set; }

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        public string RegulationNumber { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public ValueLabelModel RegisterUnit { get; set; }

        /// <summary>
        /// Hiển thị đơn vị công tác đăng ký
        /// </summary>
        public string RegistrationUnitView { get; set; }

        /// <summary>
        /// Người đại diện đăng ký
        /// </summary>
        public UserInTicketModel RegisterUser { get; set; }

        /// <summary>
        /// chức vụ
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public string LocationDevice { get; set; }

        /// <summary>
        /// Loại nội dung
        /// </summary>
        public ValueLabelModel ContentType { get; set; }

        private int _contentTypeValue;
        /// <summary>
        /// Loại nội dung
        /// </summary>
        [JsonIgnore]
        public int ContentTypeValue
        {
            get
            {
                return _contentTypeValue;
            }
            set
            {
                _contentTypeValue = value;
                switch (value)
                {
                    case 1:
                        ContentType = new ValueLabelModel { Value = value, Label = "Sửa chữa" };
                        break;
                    case 2:
                        ContentType = new ValueLabelModel { Value = value, Label = "Thí nghiệm" };
                        break;
                    case 3:
                        ContentType = new ValueLabelModel { Value = value, Label = "Xây dựng" };
                        break;
                    default:
                        ContentType = new ValueLabelModel();
                        break;
                }
            }
        }

        /// <summary>
        /// Nhận diện mối nguy và biện pháp an toàn
        /// </summary>
        public virtual List<IdentificationAndSafetyMeasuresModel> IdentificationAndSafetyMeasures { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public string OtherContents { get; set; }

        /// <summary>
        /// True: hằng ngày --  False: Liên tục trong thời gian đăng ký
        /// </summary>
        public bool IsWorkDaily { get; set; }

        /// <summary>
        /// Giờ bắt đầu hằng ngày
        /// </summary>
        public string StartTimeDaily { get; set; }

        /// <summary>
        /// Chọn thời gian làm việc hàng ngày
        /// </summary>
        public string EndTimeDaily { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Địa điểm hoặc thiết bị tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Số lượng đơn vị công tác/Số lượng nhân viên đơn vị công tác
        /// </summary>
        public string UnitNumberAndUnitStaff { get; set; }

        /// <summary>
        /// Người lãnh đạo công việc (nếu có)
        /// </summary>
        public StakeholdersResponseModel LeaderUser { get; set; }

        /// <summary>
        /// Người giám sát an toàn điện (nếu có)
        /// </summary>
        public virtual List<StakeholdersResponseModel> Supervisors { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp (require)
        /// </summary>
        public virtual List<StakeholdersResponseModel> Commanders { get; set; }

        /// <summary>
        /// Người cảnh giới (Nếu có)
        /// </summary>
        public StakeholdersResponseModel GuardUser { get; set; }

        /// <summary>
        /// Danh sách những người được cử để thực hiện công việc
        /// </summary>
        public virtual List<StakeholdersResponseModel> Workers { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<ContentPositionModel> ContentPositions { get; set; }

        /// <summary>
        /// Các đơn vị QLVH khác có liên quan
        /// </summary>
        public virtual List<string> Units { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public virtual List<OtherRequirementsModel> OtherRequirements { get; set; }

        /// <summary>
        /// Trạng thái phiếu đăng ký
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }

        /// <summary>
        /// Danh sách trạng thái của Ticket
        /// </summary>
        public virtual List<TicketStatusHistoryModel> TicketStatusHistories { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<AttachmentModel> FileAttachments { get; set; }

        /// <summary>
        /// danh sách id file đính kèm
        /// </summary>
        public string Attackments { get; set; }

        /// <summary>
        /// 1/ là phiếu đăng ký công tác - 2/ phiếu đăng ký công tác thuỷ cơ nhiệt hoá
        /// </summary>
        public int RegistrationTicketType { get; set; }

        /// <summary>
        /// Id phiếu công tác
        /// </summary>
        public int? WorkTicketID { get; set; }

        /// <summary>
        /// Id đợt sửa chữa lớn
        /// </summary>
        [JsonIgnore]
        public int? MajorRepairID { get; set; }

        /// <summary>
        /// Id phiếu yêu cầu
        /// </summary>
        [JsonIgnore]
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Thông tin phiếu công tác
        /// </summary>
        public BasicDetailModel WorkTicketInfo { get; set; }

        /// <summary>
        /// Thông tin đợt sửa chữa lớn
        /// </summary>
        public BasicDetailModel MajorRepairInfo { get; set; }

        /// <summary>
        /// Thông tin phiếu yêu cầu
        /// </summary>
        public BasicDetailModel RequestTicketInfo { get; set; }

        /// <summary>
        /// Thông tin phiếu công tác đã xóa
        /// </summary>
        public BasicDetailModel WorkTicketDeleteInfo { get; set; }

        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int? InspectionReportID { get; set; }

        /// <summary>
        ///BBKSHT
        /// </summary>
        public ValueLabelModel InspectionReport { get; set; }
    }
}
