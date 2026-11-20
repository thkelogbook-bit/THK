namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin file upload
    /// </summary>
    public class MajorRepairUploadInfoModel
    {
        /// <summary>
        /// ID SCL
        /// </summary>
        public int MajorRepairID { get; set; }

        /// <summary>
        /// Id User
        /// </summary>
        public int CurrentUserID { get; set; }

        /// <summary>
        /// LocalFileName
        /// </summary>
        public string LocalFileName { get; set; }

        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// FilePath
        /// </summary>
        public string FilePath { get; set; }
    }
}
