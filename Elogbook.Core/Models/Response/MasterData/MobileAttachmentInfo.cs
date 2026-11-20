namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tệp đính kèm
    /// </summary>
    public class MobileAttachmentInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Đường dẫn vật lý lưu tập tin
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Đường dẫn vật lý lưu tập tin thu nhỏ
        /// </summary>
        public string PathThumbnail { get; set; }

        /// <summary>
        /// Tên tập tin
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Kiểu tập tin
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Nội dung mô tả về tập tin
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Vị trí hiển thị tập tin
        /// </summary>
        public string ScreenName { get; set; }
    }
}
