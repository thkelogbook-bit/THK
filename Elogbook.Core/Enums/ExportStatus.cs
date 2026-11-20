using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Export status for export operation diary
    /// </summary>
    public enum ExportStatus
    {
        /// <summary>
        /// Đang xử lý
        /// </summary>
        [Description("Đang xử lý")]
        Waiting,

        /// <summary>
        /// Thành công
        /// </summary>
        [Description("Thành công")]
        Success,

        /// <summary>
        /// Thất bại
        /// </summary>
        [Description("Thất bại")]
        Failed,
    }
}
