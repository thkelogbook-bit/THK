namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// UploadContentFileModel
    /// </summary>
    public class UploadContentFileModel
    {
        /// <summary>
        /// Tên MH (RegistrationTicket/phiếu đăng ký công tác,\n WorkTicket/quản lý phiếu công tác,\n OperationDiary: Nhật ký vận hành)
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// nội dung file bằng base64
        /// </summary>
        public string FileBase64 { get; set; }

        /// <summary>
        /// Loại file
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; }
    }
}
