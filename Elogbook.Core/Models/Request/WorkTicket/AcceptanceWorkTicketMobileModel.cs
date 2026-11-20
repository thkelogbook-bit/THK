namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu công tác
    /// </summary>
    public class AcceptanceWorkTicketMobileModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int ApprovedUnitID { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày cấp phiếu Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }
    }
}
