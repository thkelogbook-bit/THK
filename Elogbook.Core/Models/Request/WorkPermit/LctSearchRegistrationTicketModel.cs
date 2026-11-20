using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Thông tin tìm danh sách đăng ký lệnh công tác
    /// </summary>
    public class LctSearchRegistrationTicketModel : BaseListFilter
    {
        /// <summary>
        /// màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Đơn vị đăng ký
        /// </summary>
        public int RegisterUnitID { get; set; }

        /// <summary>
        /// Ngày đăng ký (Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày đăng ký (Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Số lệnh đăng ký công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Loại lệnh (LCT điện) / Lý do (LCT thuỷ cơ nhiệt hoá)
        /// Loại lệnh: -1/Tất cả 1/kế hoạch 2/Ngoài kế hoạch 3/Xử lý sự cố
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Trạng thái lệnh đăng ký
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// Người đăng ký
        /// </summary>
        public string RegisterUser { get; set; }

        /// <summary>
        /// 1/ PCT điện - 2/ PCT thuỷ cơ nhiệt hoá
        /// </summary>
        public int? RegistrationTicketType { get; set; }
    }
}
