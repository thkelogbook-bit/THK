using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Từ chối cấp phiếu 
    /// </summary>
    public class TicketStatusHistoryModel: EntityBaseModel
    {
        /// <summary>
        /// Người tạo
        /// </summary>
        public new ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public new DateTime InsertedAt { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public ValueLabelModel RegistrationTicket { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public ValueLabelModel WorkTicket { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Lý do huỷ
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public List<AttachmentModel> FileAttachs { get; set; }
    }
}
