using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Báo cáo yêu cầu bảo trì - Chi tiết từng theo nhà máy
    /// </summary>
    public class RequestAmDetailModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Thông tin của tất cả các tháng
        /// </summary>
        public List<RequestAmByMonthModel> MonthDetails { get; set; }

        /// <summary>
        /// Tổng của số yêu cầu của 1 nhà máy
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// Tháng
        /// </summary>
        public string Months { get; set; }
    }

    /// <summary>
    /// Báo cáo yêu cầu bảo trì - Theo tháng
    /// </summary>
    public class RequestAmByMonthModel
    {
        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Tổng số yêu cầu
        /// </summary>
        public int TotalRequest { get; set; }

        /// <summary>
        /// Tổng số Chưa gửi phiếu (Phiếu công tác ở trạng thái Chưa gửi phiếu)
        /// </summary>
        public int TotalUnSent { get; set; }

        /// <summary>
        /// Tổng số Đang xử lý (Phiếu công tác ở trạng thái Đang xử lý or Từ chối)
        /// </summary>
        public int TotalOnGoing { get; set; }

        /// <summary>
        /// Tổng số Dừng xử lý (Phiếu công tác ở trạng thái Dừng xử lý)
        /// </summary>
        public int TotalStop { get; set; }

        /// <summary>
        /// Tổng số Đã duyệt (Phiếu công tác ở trạng thái Chờ đăng ký or Đã đăng ký)
        /// </summary>
        public int TotalApproved { get; set; }
    }
}
