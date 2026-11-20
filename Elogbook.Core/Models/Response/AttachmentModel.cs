namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// AttachmentModel
    /// </summary>
    public class AttachmentModel : EntityBaseModel
    {
        /// <summary>
        /// đường dẫn file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// đường dẫn file Thumbnail
        /// </summary>
        public string PathThumbnail { get; set; }

        /// <summary>
        /// tên file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// loại file
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// tên màn hình chứa file đính kèm
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Giá trị module liên kết với tập tin đính kèm
        /// </summary>
        public string ModuleValue { get; set; }

        /// <summary>
        /// Kết quả trả về có đang bị lỗi không
        /// </summary>
        public bool IsError { get; set; }
    }
}
