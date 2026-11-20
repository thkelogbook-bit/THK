using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Chức vụ những bên liên quan
    /// </summary>
    public enum RolesStakeholder
    {
        /// <summary>
        /// Giám sát
        /// </summary>
        [Description("Giám sát")]
        Supervisor = 1,

        /// <summary>
        /// Chỉ huy
        /// </summary>
        [Description("Chỉ huy")]
        Commander = 2,

        /// <summary>
        /// Công nhân
        /// </summary>
        [Description("Công nhân")]
        Worker = 3,

        /// <summary>
        /// Lãnh đạo
        /// </summary>
        [Description("Lãnh đạo")]
        Leader = 4,

        /// <summary>
        /// Cảnh giới
        /// </summary>
        [Description("Cảnh giới")]
        Guard = 5,

        /// <summary>
        /// Những người tham gia công việc
        /// </summary>
        WorkParticipants,
    }
}
