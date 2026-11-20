
namespace Elogbook.Core.Models.Response.WorkTicket.PDF881
{
    /// <summary>
    /// 
    /// </summary>
    public class Pdf881HeaderRegistrationTicketModel
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
