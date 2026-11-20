using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại sự kiện đồng bộ dữ liệu sang tồn tại ca của NKVH
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Cấp phiếu công tác
        /// </summary>
        [Description("Cấp phiếu công tác")]
        PCTD_APPROVED = 1,
        /// <summary>
        /// Cho phép thực hiện phiếu công tác
        /// </summary>
        [Description("Cho phép thực hiện phiếu công tác")]
        PCTD_ALLOWED = 2,
        /// <summary>
        /// Bắt đầu công tác hàng ngày
        /// </summary>
        [Description("Bắt đầu công tác hàng ngày")]
        PCTD_START = 3,
        /// <summary>
        /// Kết thúc công tác hàng ngày
        /// </summary>
        [Description("Kết thúc công tác hàng ngày")]
        PCTD_END = 4,
        /// <summary>
        /// Khóa phiếu
        /// </summary>
        [Description("Khóa phiếu")]
        PCTD_LOCKED = 5,
        /// <summary>
        /// Hoàn thành phiếu công tác
        /// </summary>
        [Description("Hoàn thành phiếu công tác")]
        PCTD_COMPLETED = 6,
        /// <summary>
        /// Cấp phiếu công tác
        /// </summary>
        [Description("Cấp phiếu công tác")]
        PCT_TCNH_APPROVED = 7,
        /// <summary>
        /// Cho phép thực hiện phiếu công tác
        /// </summary>
        [Description("Cho phép thực hiện phiếu công tác")]
        PCT_TCNH_ALLOWED = 8,
        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        [Description("Kết thúc công tác")]
        PCT_TCNH_COMPLETED = 9,
    }
}
