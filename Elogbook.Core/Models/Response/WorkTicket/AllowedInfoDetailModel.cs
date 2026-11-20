using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cho phép của phiếu công tác
    /// </summary>
    public class AllowedInfoDetailModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ghi chú
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
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách id file ảnh
        /// </summary>
        public string ListPictureID { get; set; }

        /// <summary>
        /// Danh sách file ảnh
        /// </summary>
        public virtual List<AttachmentModel> PictureAttachments { get; set; }
    }
}
