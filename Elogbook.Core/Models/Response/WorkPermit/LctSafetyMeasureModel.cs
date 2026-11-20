using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// các thông tin biện pháp an toàn
    /// </summary>
    public class LctSafetyMeasureModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// STT
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// Nội dung cho phép
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Chi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public string AttachmentsID { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> PictureAttachments { get; set; }
    }
}
