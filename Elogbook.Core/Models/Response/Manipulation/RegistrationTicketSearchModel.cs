using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class RegistrationTicketSearchModel : EntityBaseModel
    {
        /// <summary>
        /// Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreatedDate { get; set; }

        private DateTime _createdDateLocal;
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedDateLocal
        {
            get { return _createdDateLocal; }
            set
            {
                _createdDateLocal = value;
                if (_createdDateLocal != DateTime.MinValue)
                    CreatedDate = _createdDateLocal.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public ValueLabelModel ApproveUnit { get; set; }

        /// <summary>
        /// Tên hiển thị đơn vị cấp phiếu
        /// </summary>
        public string ApproveUnitView { get; set; }

        /// <summary>
        /// Người viết phiếu
        /// </summary>
        public ValueLabelModel CreatedTicket { get; set; }

        /// <summary>
        /// Người duyệt phiếu
        /// </summary>
        public ValueLabelModel ApprovedTicket { get; set; }

        private int _ticketTypeValue;
        /// <summary>
        /// Mã loại phiếu
        /// </summary>
        public int TicketTypeValue
        {
            get
            {
                return _ticketTypeValue;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        TicketType = new ValueLabelModel { Value = value, Label = "Kế hoạch" };
                        break;
                    case 2:
                        TicketType = new ValueLabelModel { Value = value, Label = "Đột xuất" };
                        break;
                    default:
                        TicketType = null;
                        break;
                }
                _ticketTypeValue = value;
            }
        }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }
    }
}
