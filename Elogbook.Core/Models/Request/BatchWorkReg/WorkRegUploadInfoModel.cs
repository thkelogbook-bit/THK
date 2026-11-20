namespace Elogbook.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkRegUploadInfoModel
    {
        /// <summary>
        /// Loại phiếu đăng ký công tác (1:Điện, 2 Thủy cơ nhiệt hóa)
        /// </summary>
        public int RegisType { get; set; }

        /// <summary>
        /// ID SCL
        /// </summary>
        public int BatchWorkRegID { get; set; }

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
