namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phiếu công tác chờ cấp
    /// </summary>
    public class WaitingNumberTicketModel
    {
        /// <summary>
        /// Số lượng phiếu công tác điện chờ cấp
        /// </summary>
        public int PCTD { get; set; }

        /// <summary>
        /// Số lương phiếu công tác thủy cơ nhiệt hóa chờ cấp
        /// </summary>
        public int PCT_TCNH { get; set; }

        /// <summary>
        /// Số lượng phiếu thao tác chờ cấp
        /// </summary>
        public int PTT { get; set; }

        /// <summary>
        /// Tổng số phiếu Công tác Điện (NCP)
        /// </summary>
        public int Total_PCT_Dien { get; set; }

        /// <summary>
        /// Tổng số phiếu Công tác TCNH (NCP)
        /// </summary>
        public int Total_PCT_TCNH { get; set; }

        /// <summary>
        /// Tổng số phiếu Công tác Điện (Chờ đăng ký)
        /// </summary>
        public int Total_PCT_Dien_CDK { get; set; }

        /// <summary>
        /// Tổng số phiếu Công tác TCNH (Chờ đăng ký)
        /// </summary>
        public int Total_PCT_TCNH_CDK { get; set; }
    }
}
