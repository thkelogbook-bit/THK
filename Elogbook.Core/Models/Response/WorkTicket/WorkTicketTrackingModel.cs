using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Return WorkTicketTracking Model
    /// </summary>
    public class WorkTicketTrackingModel
    {
        /// <summary>
        /// Mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tính năng
        /// </summary>
        public ValueLabelModel WorkTicket { get; set; }

        /// <summary>
        /// Người cho phép trước khi thay đổi
        /// </summary>
        public ValueLabelModel RequesterUser { get; set; }

        /// <summary>
        /// Người cho phép cần thay đổi
        /// </summary>
        public ValueLabelModel NewAllowerUser { get; set; }

        /// <summary>
        /// Người cho phép hiện tại
        /// </summary>
        public ValueLabelModel CurrentAllowerUser { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Thời gian
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }
    }
}
