using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Phân loại tổ chức
    /// </summary>
    public enum OrganizationType
    {
        /// <summary>
        /// Công ty
        /// </summary>
        [Description("Công ty")]
        CT = 1,
        /// <summary>
        /// Phân xưởng
        /// </summary>
        [Description("Phân xưởng")]
        PX,
        /// <summary>
        /// Tổ
        /// </summary>
        [Description("Tổ")]
        TO,
        /// <summary>
        /// Đội/nhóm
        /// </summary>
        [Description("Đội/nhóm")]
        DN,
    }
}
