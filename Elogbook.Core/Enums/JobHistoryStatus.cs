using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái
    /// </summary>
    public enum JobHistoryStatus
    {
        /// <summary>
        /// đang xử lý
        /// </summary>
        [Description("Đang xử lý")]
        IN_PROGRES = 1,
        /// <summary>
        /// Thành công
        /// </summary>
        [Description("Thành công")]
        SUCCESS = 2,
        /// <summary>
        /// Lỗi
        /// </summary>
        [Description("Lỗi")]
        FAIL = 3,
    }
}
