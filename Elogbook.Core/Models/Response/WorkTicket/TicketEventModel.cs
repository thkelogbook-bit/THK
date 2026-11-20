using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// TicketEvent Response
    /// </summary>
    public class TicketEventModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Thông tin sự kiện
        /// </summary>
        public ValueLabelModel DataTypeInfo { get; set; }

        /// <summary>
        /// ID loại sự kiện
        /// </summary>
        public int EventTypeID { get; set; }

        /// <summary>
        /// Thông tin sự kiện
        /// </summary>
        public ValueLabelModel EventType { get; set; }

        /// <summary>
        /// Nội dung mẫu
        /// </summary>
        public string EventContent { get; set; }

        /// <summary>
        /// Dữ liệu hệ thống
        /// </summary>
        public bool IsSystem { get; set; }
    }
}
