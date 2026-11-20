using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Bậc an toàn điện
    /// </summary>
    public enum ElectricRank
    {
        /// <summary>
        /// Bậc 1
        /// </summary>
        [Description("1/5")]
        Level1 = 1,
        /// <summary>
        /// Bậc 2
        /// </summary>
        [Description("2/5")]
        Level2,
        /// <summary>
        /// Bậc 3
        /// </summary>
        [Description("3/5")]
        Level3,
        /// <summary>
        /// Bậc 4
        /// </summary>
        [Description("4/5")]
        Level4,
        /// <summary>
        /// Bậc 5
        /// </summary>
        [Description("5/5")]
        Level5,
    }
}
