using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại
    /// </summary>
    public enum EnumHazardsAndSafetyMeasures
    {
        /// <summary>
        /// Mối nguy hiểm
        /// </summary>
        [Description("Mối nguy hiểm")]
        Hazards = 1,

        /// <summary>
        /// Biện pháp an toàn
        /// </summary>
        [Description("Biện pháp an toàn")]
        SafetyMeasures
    }
}
