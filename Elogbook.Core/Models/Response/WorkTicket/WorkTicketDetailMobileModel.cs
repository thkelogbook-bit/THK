using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Quản lý phiếu công tác
    /// </summary>
    public class WorkTicketDetailMobileModel : EntityBaseModel
    {
        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public ValueLabelModel WorkTicketStatus { get; set; }

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
        /// Người lãnh đạo
        /// </summary>
        public LeadershipUserModel LeaderUser { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LeadershipUserModel AcceptorUser { get; set; }

        /// <summary>
        /// Nhân viên đơn vị công tác
        /// </summary>
        public int UnitNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Thời gian theo kế hoạch
        /// </summary>
        public string PlanTime { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Phạm vi công việc
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Tính năng của phiếu (Loại phiếu)
        /// </summary>
        public ValueLabelModel WorkTicketFeature { get; set; }
        /// <summary>
        /// Loại Phiếu
        /// </summary>
        public List<WorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Nhận diện mối nguy hiểm và biện pháp an toàn
        /// </summary>
        public List<WarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public LeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public AllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tổ chức thực hiện
        /// </summary>
        public ReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public RegistrationDetailModel RegistrationDetails { get; set; }

        /// <summary>
        /// Kết quả công tác
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> WorkPictures { get; set; }

        /// <summary>
        /// id phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }
    }
}
