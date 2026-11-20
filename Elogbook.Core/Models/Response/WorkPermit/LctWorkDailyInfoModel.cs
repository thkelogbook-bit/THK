using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class LctWorkDailyInfoModel
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

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public string CommanderSignatureContent { get; set; }

        /// <summary>
        /// Chữ ký Người giám sát 
        /// </summary>
        public string SupervisorSignatureContent { get; set; }

    }
}
