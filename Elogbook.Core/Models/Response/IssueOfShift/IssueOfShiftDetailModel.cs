using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tồn tại trong ca
    /// </summary>
    public class IssueOfShiftDetailModel
    {
        /// <summary>
        /// Mã tồn tại
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreateDateForWeb { get; set; }

        /// <summary>
        /// Trạng thái của tồn tại
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// Nội dung tồn tại
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
        public Boolean IsEditable { get; set; }
    }
}
