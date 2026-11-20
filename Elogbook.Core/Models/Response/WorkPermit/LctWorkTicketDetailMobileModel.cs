using Elogbook.Core.Constants;
using Elogbook.Core.Models.Request.WorkPermit;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Quản lý lệnh công tác
    /// </summary>
    public class LctWorkTicketDetailMobileModel : EntityBaseModel
    {
        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Ngày cấp lệnh
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh
        /// </summary>
        public ValueLabelModel ApprovedUnit { get; set; }

        /// <summary>
        /// Trạng thái lệnh
        /// </summary>
        public ValueLabelModel WorkTicketStatus { get; set; }

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
        /// Người lãnh đạo
        /// </summary>
        public LctLeadershipUserModel LeaderUser { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LctLeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Người chỉ huy công tác hàng ngày
        /// </summary>
        public LctLeadershipUserModel CommanderWorkDailyUser { get; set; }

        /// <summary>
        /// Người chỉ huy kết thúc công tác
        /// </summary>
        public LctLeadershipUserModel CommanderWorkkFinishUser { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LctLeadershipUserModel AcceptorUser { get; set; }

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
        /// Tính năng của lệnh (Loại lệnh)
        /// </summary>
        public ValueLabelModel WorkTicketFeature { get; set; }
        /// <summary>
        /// Loại lệnh
        /// </summary>
        public List<LctWorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Nhận diện mối nguy hiểm và biện pháp an toàn
        /// </summary>
        public List<LctWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người cấp lệnh
        /// </summary>
        public LctLeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// Người cấp lệnh tổ chức thực hiện (Dành cho công tác Điện)
        /// </summary>
        public LctLeadershipUserModel ExecutingOrgApproverUser { get; set; }
        
        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public LctAllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tổ chức thực hiện
        /// </summary>
        public LctReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public LctRegistrationDetailModel RegistrationDetails { get; set; }

        /// <summary>
        /// Kết quả công tác
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> WorkPictures { get; set; }

        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        public List<LctStakeholdersResponseModel> WorkerUsers { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<LctContentPositionModel> ContentPositions { get; set; }

    }
}
