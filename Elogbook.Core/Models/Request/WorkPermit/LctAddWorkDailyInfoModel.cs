
namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class LctAddWorkDailyInfoModel
    {
        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string CommanderNameElectricRank { get; set; }

        /// <summary>
        /// Người giám sát ATĐ
        /// </summary>
        public string SupervisorName { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string SupervisorUnit { get; set; }

    }
}
