namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// ContentModel
    /// </summary>
    public class LctPdf881ContentModel
    {
        /// <summary>
        /// Header
        /// </summary>
        public LctPdf881HeaderModel Header { get; set; }

        /// <summary>
        /// PhaseA
        /// </summary>
        public LctPdf881PhaseAModel PhaseA { get; set; }

        /// <summary>
        /// PhaseB
        /// </summary>
        //public LctPdf881PhaseBModel PhaseB { get; set; }

        /// <summary>
        /// PhaseB
        /// </summary>
        public LctPdf881PhaseBModel PhaseB { get; set; }

        /// <summary>
        /// PhaseC
        /// </summary>
        public LctPdf881PhaseCModel PhaseC { get; set; }

    }
}
