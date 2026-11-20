namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// CountNumberTicketStatusModel
    /// </summary>
    public class CountNumberTicketStatusModel
    {
        /// <summary>
        /// Tổng số phiếu ở trạng thái đang xử lý với PYC trưởng ca
        /// </summary>
        public int Total_Shift_Leader { get; set; }

        /// <summary>
        /// Tổng số phiếu ở trạng thái đã duyệt  với PYC đơn vị sửa chữa
        /// </summary>
        public int Total_DVSC { get; set; }
    }
}
