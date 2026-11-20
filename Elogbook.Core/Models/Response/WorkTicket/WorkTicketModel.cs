using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số trả về phiếu công tác
    /// </summary>
    public class WorkTicketModel : EntityBaseModel
    {
        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        private DateTime? _approvedDateLocal;
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [JsonIgnore]
        public DateTime? ApprovedDateLocal
        {
            get { return _approvedDateLocal; }
            set
            {
                _approvedDateLocal = value;
                if (_approvedDateLocal != null && _approvedDateLocal != DateTime.MinValue)
                    ApprovedDate = _approvedDateLocal.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Tính năng
        /// </summary>
        public ValueLabelModel WorkTicketFeature { get; set; }

        /// <summary>
        /// Tính năng
        /// </summary>
        public List<WorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu (cấp phân xưởng)
        /// </summary>
        public ValueLabelModel ApproveUnit { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public ValueLabelModel Approver { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public ValueLabelModel Acceptor { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public ValueLabelModel WorkTicketType { get; set; }

        private int _workTicketTypeValue;
        /// <summary>
        /// Loại phiếu
        /// </summary>
        [JsonIgnore]
        public int WorkTicketTypeValue
        {
            get { return _workTicketTypeValue; }
            set
            {
                _workTicketTypeValue = value;
                switch (_workTicketTypeValue)
                {
                    case (int)TicketType.InsidePlan:
                        WorkTicketType = new ValueLabelModel { Value = _workTicketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketType.InsidePlan) };
                        break;
                    case (int)TicketType.OutsidePlan:
                        WorkTicketType = new ValueLabelModel { Value = _workTicketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketType.OutsidePlan) };
                        break;
                    case (int)TicketType.SpecialCase:
                        WorkTicketType = new ValueLabelModel { Value = _workTicketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketType.SpecialCase) };
                        break;
                    case (int)TicketType.MajorRepairCase:
                        WorkTicketType = new ValueLabelModel { Value = _workTicketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketType.MajorRepairCase) };
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
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime? ApprovedDateStyleDatetime { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// Trạng thái thay đổi người cho phép
        /// </summary>
        public ValueLabelModel TrackingStatus { get; set; }

        /// <summary>
        /// Thông tin đợt công tác sửa chữa lớn
        /// </summary>
        public ValueLabelModel MajorRepair { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public ValueLabelModel RegistrationTicket { get; set; }

        /// <summary>
        /// Người tạo phiếu
        /// </summary>
        public ValueLabelModel CreaterUser { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Id Phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        private DateTime? _completedTimeLocal;

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        [JsonIgnore]
        public DateTime? CompletedTimeLocal
        {
            get { return _completedTimeLocal; }
            set
            {
                _completedTimeLocal = value;
                if (_completedTimeLocal != null && _completedTimeLocal != DateTime.MinValue)
                    CompletedTime = _completedTimeLocal.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS);
            }
        }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string CompletedTime { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Loại phiếu (Điện, TCNH)
        /// </summary>
        public int? ScreenType { get; set; }
    }
}
