
namespace Elogbook.Core.Models.Response.WorkTicket.PDF
{
    /// <summary>
    /// Tham số tạo Header File PDF WorkTicket
    /// </summary>
    public class PdfHeaderRegistrationTicketModel
    {

        /// <summary>
        /// Số phiếu đăng ký công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Đơn vị nhận phiếu
        /// </summary>
        public string ReceiveUnit { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public string RegisterUnit { get; set; }
    }
}
