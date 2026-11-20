namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin nguy hiểm và biện pháp an toàn
    /// </summary>
    public class LctWarnAndSafeModel
    {
        /// <summary>
        /// STT
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// nguy hiểm
        /// </summary>
        public string WarningContent { get; set; }

        /// <summary>
        /// biện pháp an toàn
        /// </summary>
        public string SafetyMeasureContent { get; set; }
    }
}
