using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái đợt công tác sửa chữa lớn
    /// </summary>
    public enum MajorRepairStatusDefine
    {
        /// <summary>
        /// Chưa thực hiện cấp phiếu
        /// </summary>
        [Description("Chưa thực hiện cấp phiếu")]
        Waiting = 1,
        /// <summary>
        /// Đang cấp phiếu hàng loạt
        /// </summary>
        [Description("Đang cấp phiếu hàng loạt")]
        Doing,
        /// <summary>
        /// Hoàn thành cấp phiếu
        /// </summary>
        [Description("Hoàn thành cấp phiếu")]
        Finish,
        /// <summary>
        /// Có lỗi trong quá trình xử lý
        /// </summary>
        [Description("Có lỗi trong quá trình xử lý")]
        Error,
    }
}
