namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// UploadFileModel
    /// </summary>
    public class UploadFileModel : EntityBaseModel
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
        /// mã số file upload
        /// </summary>
        public new int ID { get; set; }

        /// <summary>
        /// mô tả nếu có error
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// loại file
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Kết quả trả về có đang bị lỗi không
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Thông tin lỗi
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
