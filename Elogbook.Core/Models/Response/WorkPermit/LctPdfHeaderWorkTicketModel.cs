namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Tham số tạo Header File PDF WorkTicket
    /// </summary>
    public class LctPdfHeaderWorkTicketModel
    {
        /// <summary>
        /// Tên công ty
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh
        /// </summary>
        public string ApprovedUnit { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

    }
}
