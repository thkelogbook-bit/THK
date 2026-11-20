
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    public class LctOrderExecutionMobileModel
    {
        /// <summary>
        /// Người chỉ huy trức tiếp
        /// </summary>
        public LctLeadershipUserModel CommanderExecutionUser { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public LctLeadershipUserModel SupervisorExecutionUser { get; set; }

        /// <summary>
        /// Trình tự công việc và điều kiện an toàn
        /// </summary>
        public List<WorkProcessSafetyMobileModel> WorkProcessSafeties { get; set; }
    }
}
