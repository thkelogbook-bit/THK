using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// thông tin lệnh đăng ký công tác
    public class LctWorkTicketWaitingModel
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
        public string RegistrationDate { get; set; }

        /// <summary>
        /// Dùng để sắp xếp lệnh theo ngày tháng
        /// </summary>
        [JsonIgnore]
        public DateTime? RegistrationDateSort { get; set; }

        private DateTime? _registrationDateTemp;
        /// <summary>
        /// Vị trí
        /// </summary>
        [JsonIgnore]
        public DateTime? RegistrationDateTemp
        {
            get
            {
                return _registrationDateTemp;
            }
            set
            {
                _registrationDateTemp = value;

                if (_registrationDateTemp != null && _registrationDateTemp.Value != DateTime.MinValue)
                {
                    RegistrationDate = _registrationDateTemp.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    RegistrationDate = string.Empty;
                }
            }
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public ValueLabelModel RegisterUnit { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public ValueLabelModel ParentUnit { get; set; }

        /// <summary>
        /// Người đại diện đăng ký
        /// </summary>
        public ValueLabelModel RegisterUser { get; set; }

        /// <summary>
        /// Loại lệnh
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        /// <summary>
        /// Dùng để sắp xếp trạng thái lệnh
        /// </summary>
        [JsonIgnore]
        public int TicketTypeSort { get; set; }

        private int _ticketTypeValue;
        /// <summary>
        /// Loại lệnh (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
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
        public string FromDate { get; set; }

        private DateTime? _fromDateTemp;
        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        [JsonIgnore]
        public DateTime? FromDateTemp
        {
            get
            {
                return _fromDateTemp;
            }
            set
            {
                _fromDateTemp = value;

                if (_fromDateTemp != null && _fromDateTemp.Value != DateTime.MinValue)
                {
                    FromDate = _fromDateTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    FromDate = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public string ToDate { get; set; }

        private DateTime? _toDateTemp;
        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        [JsonIgnore]
        public DateTime? ToDateTemp
        {
            get
            {
                return _toDateTemp;
            }
            set
            {
                _toDateTemp = value;

                if (_toDateTemp != null && _toDateTemp.Value != DateTime.MinValue)
                {
                    ToDate = _toDateTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    ToDate = string.Empty;
                }
            }
        }
    }
}
