
namespace Elogbook.Core.Models.Response.WorkTicket.PDF881
{
    /// <summary>
    /// 
    /// </summary>
    public class Pdf881RegistrationTicketModel
    {
        /// <summary>
        /// Thông tin tiêu đề của file PDF
        /// </summary>
        public Pdf881HeaderRegistrationTicketModel PdfHeader { get; set; }

        /// <summary>
        /// Thông tin của file PDF
        /// </summary>
        public Pdf881InfoRegistrationTicketModel PdfInfo { get; set; }
    }
}
