namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số tạo Header File PDF WorkTicket
    /// </summary>
    public class PdfHeaderWorkTicketModel
    {
        /// <summary>
        /// Tên công ty
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public string ApprovedUnit { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

    }
}
