using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm một biện pháp
    /// </summary>
    public class AddWorkFinishPictureModel
    {
        /// <summary>
        /// Danh sách ID hình ảnh
        /// </summary>
        public List<string> Attachments { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }
    }
}
