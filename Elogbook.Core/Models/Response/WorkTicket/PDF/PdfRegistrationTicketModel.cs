
namespace Elogbook.Core.Models.Response.WorkTicket.PDF
{
    /// <summary>
    /// Tham số tạo file template RegistrationTicket
    /// </summary>
    public class PdfRegistrationTicketModel
    {
        /// <summary>
        /// Thông tin tiêu đề của file PDF
        /// </summary>
        public PdfHeaderRegistrationTicketModel PdfHeader { get; set; }

        /// <summary>
        /// Thông tin của file PDF
        /// </summary>
        public PdfInfoRegistrationTicketModel PdfInfo { get; set; }

    }
}
