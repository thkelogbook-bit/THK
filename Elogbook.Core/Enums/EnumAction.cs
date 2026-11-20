using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Action record
    /// </summary>
    public enum EnumAction
    {
        /// <summary>
        /// Thêm mới
        /// </summary>
        [Description("Thêm mới")]
        Insert = 1,
        /// <summary>
        /// Cập nhật
        /// </summary>
        [Description("Cập nhật")]
        Update,
        /// <summary>
        /// Xóa
        /// </summary>
        [Description("Xóa")]
        Delete,
    }
}
