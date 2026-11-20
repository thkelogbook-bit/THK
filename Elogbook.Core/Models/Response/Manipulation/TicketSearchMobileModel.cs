using Elogbook.Core.Models.Response.Manipulation;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class TicketSearchMobileModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        public ValueLabelModel TicketType { get; set; }
        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public ValueLabelModel TicketStatus { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Ngày duyệt
        /// </summary>
        public DateTime ApprovedDate { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// 1.1 Người tạo phiếu
        /// </summary>
        public ParticipantModel CreatedUser { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt
        /// </summary>
        public ParticipantModel ApprovedUser { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt với ID
        /// </summary>
        public int ApprovedUserID { get; set; }

    }
}
