using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Các lỗi thống số vận hành
    /// </summary>
    public enum DeviceErrorCode
    {
        /// <summary>
        /// Thiết bị bị hỏng (F)
        /// </summary>
        [Description("Thiết bị bị hỏng (F)")]
        Error = 1,
        /// <summary>
        /// Dừng thiết bị (-)
        /// </summary>
        [Description("Dừng thiết bị (-)")]
        Stop,
        /// <summary>
        /// Sim thiết bị (S)
        /// </summary>
        [Description("Sim thiết bị (S)")]
        Sim,
        /// <summary>
        /// Jump thiết bị (J)
        /// </summary>
        [Description("Jump thiết bị (J)")]
        Jump,
        /// <summary>
        /// Bận công tác (x)
        /// </summary>
        [Description("Bận công tác (x)")]
        Busy,
    }
}
