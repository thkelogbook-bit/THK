namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Table lưu dữ liệu thông tin hủy phiếu thao tác
    /// </summary>
    public class MobileCancelTicketInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Lý do hủy phiếu
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Danh sách hình ảnh/videos hủy phiếu
        /// </summary>
        public string ReferAttachmentIDs { get; set; }
    }
}
