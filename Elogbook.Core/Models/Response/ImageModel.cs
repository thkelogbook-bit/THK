namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông số của image
    /// </summary>
    public class ImageModel
    {
        /// <summary>
        /// tên file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// loại file
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// đường dẫn file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// nội dung file
        /// </summary>
        public string FileContent { get; set; }
    }
}
