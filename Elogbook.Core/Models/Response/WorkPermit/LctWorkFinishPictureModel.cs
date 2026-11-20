using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Danh sách hình ảnh kết thúc công tác
    /// </summary>
    public class LctWorkFinishPictureModel
    {
        /// <summary>
        /// Danh sách file hình ảnh
        /// </summary>
        public string PictureAttachmentsID { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> PictureAttachments { get; set; }
    }
}
