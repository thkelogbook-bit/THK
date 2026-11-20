using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Loại hoạt động
    /// </summary>
    public enum AmEnumDeAbStatus
    {
        /// <summary>
        /// Không có bất thường
        /// </summary>
        [Description("Không có bất thường")]
        No = 1,
        /// <summary>
        /// Có bất thường - Chưa tạo phiếu yêu cầu - PYC chưa được tạo or đã tạo mà có trạng thái Chưa gửi phiếu
        /// </summary>
        [Description("Có bất thường - Chưa tạo phiếu yêu cầu")]
        Yes_NoCreate = 2,
        /// <summary>
        /// Có bất thường - Đang xử lý - PYC đã tạo đang có trạng thái Đang xử lý or Từ chối
        /// </summary>
        [Description("Có bất thường - Đang xử lý")]
        Yes_Created = 3,
        /// <summary>
        /// Có bất thường - Dừng xử lý - PYC đã tạo và có trạng thái Dừng xử lý
        /// </summary>
        [Description("Có bất thường - Dừng xử lý")]
        Yes_Stop = 4,
        /// <summary>
        /// Có bất thường - Đã duyệt - PYC đã được duyệt và có trạng thái Chờ đăng ký trở lên
        /// </summary>
        [Description("Có bất thường - Đã duyệt")]
        Yes_Approved = 5,
        /// <summary>
        /// Có bất thường - Đang thực hiện sửa chữa - Phiếu đăng ký đã tạo - Phiếu công tác đã tạo và có trạng thái Đã cho phép trở lên
        /// </summary>
        [Description("Có bất thường - Đang thực hiện sửa chữa")]
        Yes_InProgress = 6,
        /// <summary>
        /// Có bất thường - Hoàn thành sửa chữa  - Phiếu đăng ký đã tạo - Phiếu công tác đã tạo và có trạng thái Đã hoàn thành
        /// </summary>
        [Description("Có bất thường - Hoàn thành sửa chữa")]
        Yes_Finish = 7,
    }
}
