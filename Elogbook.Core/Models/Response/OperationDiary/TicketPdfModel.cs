namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// TicketPdfModel
    /// </summary>
    public class TicketPdfModel
    {
        /// <summary>
        /// giờ
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// số phiếu công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// nhóm trưởng
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Thời gian khóa phiếu
        /// </summary>
        public string TimeBlockTicket { get; set; }

        /// <summary>
        /// kết quả
        /// </summary>
        public string Result { get; set; }
    }
}
