namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin kiểm tra tồn tại phiếu
    /// </summary>
    public class ExistTicketModel
    {
        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Loại phiếu: 1/PCTĐ, 2/PCTTCNH, 3/PTT
        /// </summary>
        public int ScreenType { get; set; }
    }
}
