namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin người dùng của ca trực
    /// </summary>
    public class PdfDeliveryReceiveShiftUserInfo
    {
        /// <summary>
        /// Họ và tên vận hành viên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }
    }
}
