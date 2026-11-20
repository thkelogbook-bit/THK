using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa trạng thái Tạo Phiếu công tác Số lượng lớn
    /// </summary>
    public enum EnumWorkRegStatus
    {
        /// <summary>
        /// Chưa thực hiện tạo đăng ký phiếu
        /// </summary>
        [Description("Chưa thực hiện tạo phiếu đăng ký hàng loạt")]
        Waiting = 1,
        /// <summary>
        /// Đang tạo phiếu đăng ký hàng loạt
        /// </summary>
        [Description("Đang tạo phiếu đăng ký hàng loạt")]
        Doing,
        /// <summary>
        /// Hoàn thành tạo phiếu đăng ký hàng loạt
        /// </summary>
        [Description("Hoàn thành tạo phiếu đăng ký hàng loạt")]
        Finish,
        /// <summary>
        /// Có lỗi trong quá trình xử lý
        /// </summary>
        [Description("Có lỗi trong quá trình xử lý")]
        Error,
    }
}
