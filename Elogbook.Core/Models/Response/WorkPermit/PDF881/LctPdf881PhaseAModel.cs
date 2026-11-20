using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    public class LctPdf881PhaseAModel
    {
        public string Title { get; set; }

        public string WorkLocaltion { get; set; }

        public string WorkContent { get; set; }

        public string WorkScope { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public List<LctWarnAndSafeModel> WarnAndSafes { get; set; }

        public string ApproverName { get; set; }

        public string ApproverSignature { get; set; }

        public string ApproveTime { get; set; }
    }
}
