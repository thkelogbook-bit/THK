using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Chức danh của cương vị trong ca
    /// </summary>
    public enum JobTitle
    {
        /// <summary>
        /// Chức danh Trưởng ca
        /// </summary>
        [Description("Trưởng ca")]
        ShiftLeader = 1,

        /// <summary>
        /// Chức danh trưởng kíp
        /// </summary>
        [Description("Trưởng kíp")]
        TeamLeader = 2,

        /// <summary>
        /// Chức danh vận hành viên
        /// </summary>
        [Description("Vận hành viên")]
        Operator = 3,
    }
}
