using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    public class LctPdf881PhaseBModeDel
    {
        public string Title { get; set; }

        public  List<LctAllowedInfoDetailModel> AllowedInfos{ get; set; }

        public string AllowerName { get; set; }

        public string AllowerSignature { get; set; }

        public string SignedTime1 { get; set; }

        public string CommanderName { get; set; }

        public string CommanderSignature { get; set; }

        public string SignedTime2 { get; set; }
    }
}
