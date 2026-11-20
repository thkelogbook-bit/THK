using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin xử lý bất thường sự cố
    /// </summary>
    public class AbnormalIncidentDetailModel
    {
        /// <summary>
        /// Mã thông tin xử lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Nội dung xử lý bất thường sự cố
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<AttachmentModel> FileAttachments { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Có thể chỉnh sửa
        /// </summary>
        public bool IsEditable { get; set; }
    }
}
