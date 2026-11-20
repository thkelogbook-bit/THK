using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Sử dụng thẻ RFID/QR Code
    /// </summary>
    public enum AthenticationType
    {
        /// <summary>
        /// Không sử dụng
        /// </summary>
        [Description("Không sử dụng")]
        NotUse = 0,
        /// <summary>
        /// RFID
        /// </summary>
        [Description("RFID")]
        RFID = 1,
        /// <summary>
        /// QR Code
        /// </summary>
        [Description("QR Code")]
        QrCode,
    }
}
