namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa các mức độ cho phép log lỗi
    /// </summary>
    public enum TrackingLogLevel
    {
        /// <summary>
        /// Tất cả các level
        /// </summary>
        ALL = 1,
        /// <summary>
        /// Level debug, 
        /// </summary>
        DEBUG,
        /// <summary>
        /// Level info
        /// </summary>
        INFO,
        /// <summary>
        /// Level warning
        /// </summary>
        WARN,
        /// <summary>
        /// Level error
        /// </summary>
        ERROR,
        /// <summary>
        /// Level fatal
        /// </summary>
        FATAL,
        /// <summary>
        /// Off
        /// </summary>
        OFF
    }
}
