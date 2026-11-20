namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Nội dung cảnh báo nguy hiểm và biện pháp an toàn
    /// </summary>
    public class LctAddWarnAndSafeModel
    {
        /// <summary>
        /// Nội dung nguy hiểm
        /// </summary>
        public string WarningContent { get; set; }

        /// <summary>
        /// Biện pphaps an toàn
        /// </summary>
        public string SafetyMeasureContent { get; set; }
    }
}
