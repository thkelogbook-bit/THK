namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// lệnh công tác chờ cấp
    /// </summary>
    public class LctWaitingNumberTicketModel
    {
        /// <summary>
        /// Số lượng lệnh công tác điện chờ cấp
        /// </summary>
        public int PCTD { get; set; }

        /// <summary>
        /// Số lương lệnh công tác thủy cơ nhiệt hóa chờ cấp
        /// </summary>
        public int PCT_TCNH { get; set; }

        /// <summary>
        /// Số lượng lệnh thao tác chờ cấp
        /// </summary>
        public int PTT { get; set; }
    }
}
