namespace Elogbook.Core.Models.Request.WorkTicket
{
    /// <summary>
    /// Không cho phép thực hiện
    /// </summary>
    public class NotAllowedModel
    {
        /// <summary>
        /// ID phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Lý do không cho phép thực hiện
        /// </summary>
        public string Reason { get; set; }
    }
}
