namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// ContentModel
    /// </summary>
    public class Pdf881ContentModel
    {
        /// <summary>
        /// Header
        /// </summary>
        public Pdf881HeaderModel Header { get; set; }

        /// <summary>
        /// PhaseA
        /// </summary>
        public Pdf881PhaseAModel PhaseA { get; set; }

        /// <summary>
        /// PhaseB
        /// </summary>
        public Pdf881PhaseBModel PhaseB { get; set; }

        /// <summary>
        /// PhaseC
        /// </summary>
        public Pdf881PhaseCModel PhaseC { get; set; }

        /// <summary>
        /// PhaseD
        /// </summary>
        public Pdf881PhaseDModel PhaseD { get; set; }

    }
}
