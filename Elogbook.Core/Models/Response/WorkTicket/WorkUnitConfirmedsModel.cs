using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Đơn vị công tác khẳng định
    /// </summary>
    public class WorkUnitConfirmedsModel
    {
        /// <summary>
        /// Tiêu đề xác nhận
        /// </summary>
        public string ConfirmedTitle { get; set; }

        /// <summary>
        /// Đã kiếm tra
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Danh sách file hình ảnh
        /// </summary>
        public string Pictures { get; set; }

        /// <summary>
        /// Danh sách hình ảnh đính kèm
        /// </summary>
        public List<AttachmentModel> PictureAttachments { get; set; }
    }
}
