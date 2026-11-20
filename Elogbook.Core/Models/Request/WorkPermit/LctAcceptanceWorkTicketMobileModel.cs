namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số tìm kiếm lệnh công tác
    /// </summary>
    public class LctAcceptanceWorkTicketMobileModel
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
        /// Ngày cấp lệnh Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày cấp lệnh Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }
    }
}
