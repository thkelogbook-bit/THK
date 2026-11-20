using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại nội dung phiếu
    /// </summary>
    public enum TicketContentType
    {
        /// <summary>
        /// Kế hoạch
        /// </summary>
        [Description("Sửa chữa")]
        Repair = 1,
        /// <summary>
        /// Ngoài kế hoạch
        /// </summary>
        [Description("Thí nghiệm")]
        Experiment,
        /// <summary>
        /// Xử lý sự cố
        /// </summary>
        [Description("Xây dựng")]
        Construct,
    }
}
