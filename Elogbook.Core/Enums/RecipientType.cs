namespace Elogbook.Core.Enums
{
    /// <summary>
    /// phân loại email được gửi tới
    /// </summary>
    public enum RecipientType
    {
        /// <summary>
        /// CC to email address
        /// </summary>
        Cc = 1,
        /// <summary>
        /// BCC to email address
        /// </summary>
        Bcc,
        /// <summary>
        /// To email address
        /// </summary>
        To
    }
}
