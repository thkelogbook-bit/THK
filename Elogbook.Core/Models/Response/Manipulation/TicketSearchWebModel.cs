using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class TicketSearchWebModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// Ngày cấp
        /// </summary>
        public string ApprovedDate { get; set; }

        private DateTime _createdDate;
        /// <summary>
        /// Ngày cấp
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                if (_createdDate != DateTime.MinValue)
                {
                    ApprovedDate = _createdDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public ValueLabelModel Company { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public ValueLabelModel ApproveUnit { get; set; }

        /// <summary>
        /// Loại phiếu
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        private int _ticketTypeValue;

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2: Không áp dụng
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
                switch (_ticketTypeValue)
                {
                    case (int)TicketTypePTT.KeHoach:
                        TicketType = new ValueLabelModel { Value = _ticketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketTypePTT.KeHoach) };
                        break;
                    case (int)TicketTypePTT.DotXuat:
                        TicketType = new ValueLabelModel { Value = _ticketTypeValue, Label = EnumExtensions.GetEnumDescription(TicketTypePTT.DotXuat) };
                        break;
                    default:
                        TicketType = new ValueLabelModel { Value = _ticketTypeValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái
        /// Chờ xác nhận hoàn thành/ Chưa thực hiện/ Đã hoàn thành/ Bị hủy
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }

    }
}
