using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin lệnh đăng ký công tác hiển thị trên lưới
    /// </summary>
    public class LctGeneralRegistrationTicketModel
    {
        /// <summary>
        /// Mã lệnh đăng ký công tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số lệnh đăng ký công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

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
        public ValueLabelModel RegisterUser { get; set; }

        /// <summary>
        /// Loại lệnh (LCT điện) / Lý do (LCT thuỷ cơ nhiệt hoá)
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        private int _ticketTypeValue;
        /// <summary>
        /// Loại lệnh (LCT điện) / Lý do (LCT thuỷ cơ nhiệt hoá)
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
        /// Trạng thái lệnh đăng ký
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public ValueLabelModel Reason { get; set; }

        /// <summary>
        /// Trạng thái lệnh đăng ký
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// 1/ Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }
    }
}
