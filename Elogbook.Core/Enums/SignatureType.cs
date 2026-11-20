using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại chữ ký
    /// </summary>
    public enum SignatureType
    {
        /// <summary>
        /// Hai hệ thống
        /// </summary>
        [Description("Ảnh chữ ký")]
        Image = 1,
        /// <summary>
        /// AdminPortal
        /// </summary>
        [Description("Ký điện tử")]
        Electric = 2,
        /// <summary>
        /// MobileApp
        /// </summary>
        [Description("Ký số")]
        Digital = 3,
    }
}
