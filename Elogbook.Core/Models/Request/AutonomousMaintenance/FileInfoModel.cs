namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin file upload
    /// </summary>
    public class FileInfoModel
    {
        /// <summary>
        /// LocalFileName
        /// </summary>
        public string LocalFileName { get; set; }

        /// <summary>
        /// File Path
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// ThumbnailFileName
        /// </summary>
        public string ThumbnailFileName { get; set; }

        /// <summary>
        /// FileExtension
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// FileType
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// CurrentUserID
        /// </summary>
        public int CurrentUserID { get; set; }
    }
}
