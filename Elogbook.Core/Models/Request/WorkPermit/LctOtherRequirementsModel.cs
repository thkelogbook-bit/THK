namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Những yêu cầu khác
    /// </summary>
    public class LctOtherRequirementsModel
    {
        /// <summary>
        /// Mã yêu cầu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Biện pháp an toàn/ Tiếp đất di động
        /// </summary>
        public string SafetyMeasures { get; set; }
    }
}
