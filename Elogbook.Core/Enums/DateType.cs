using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Kiểu ngày cần tìm kiếm
    /// </summary>
    public enum DateType
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [Description("Ngày tạo")]
        CreateDate = 1,
        /// <summary>
        /// Ngày sử dụng
        /// </summary>
        [Description("Ngày sử dụng")]
        UseDate,
    }
}
