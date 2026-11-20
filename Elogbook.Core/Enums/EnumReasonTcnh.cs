using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Lý do ( Phân loại lý do TCNH)
    /// </summary>
    public enum EnumReasonTcnh
    {
        /// <summary>
        /// Sửa chữa
        /// </summary>
        [Description("Sửa chữa")]
        Repair = 1,
        /// <summary>
        /// Thí nghiệm
        /// </summary>
        [Description("Thí nghiệm")]
        Test,
        /// <summary>
        /// Xây dựng
        /// </summary>
        [Description("Xây dựng")]
        Construction,
    }
}
