using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Tình trạng xử lý
    /// </summary>
    public enum ProcessingStatus
    {
        /// <summary>
        /// Chưa xử ký
        /// </summary>
        [Description("Chưa xử ký")]
        Waiting = 1,
        /// <summary>
        /// Đang xử lý
        /// </summary>
        [Description("Đang xử lý")]
        InProgress,
        /// <summary>
        /// Đã xử lý
        /// </summary>
        [Description("Đã xử lý")]
        Completed,
    }
}
