using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Báo cáo yêu cầu bảo trì - Mức độ khẩn cấp
    /// </summary>
    public class RequestAmReportEmergencyModel
    {
        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Chưa gửi phiếu
        /// </summary>
        public List<ReportEmergency> UnSentInfo { get; set; }

        /// <summary>
        /// Tổng Chưa gửi phiếu
        /// </summary>
        public int TotalUnSent { get; set; } = 0;

        /// <summary>
        /// Đang xử lý
        /// </summary>
        public List<ReportEmergency> OnGoingInfo { get; set; }

        /// <summary>
        /// Tổng Đang xử lý
        /// </summary>
        public int TotalOnGoing { get; set; } = 0;

        /// <summary>
        /// Dừng xử lý
        /// </summary>
        public List<ReportEmergency> StopInfo { get; set; }

        /// <summary>
        /// Tổng Dừng xử lý
        /// </summary>
        public int TotalStop { get; set; } = 0;

        /// <summary>
        /// Đã duyệt (Phiếu công tác ở trạng thái Chờ đăng ký or Đã đăng ký)
        /// </summary>
        public List<ReportEmergency> ApprovedInfo { get; set; }

        /// <summary>
        /// Tổng Đã được duyệt
        /// </summary>
        public int TotalApproved { get; set; } = 0;

        /// <summary>
        /// Tổng
        /// </summary>
        public List<ReportEmergency> TotalInfo { get; set; }

        /// <summary>
        /// Tổng Tổng
        /// </summary>
        public int TotalTotal { get; set; } = 0;

        /// <summary>
        /// Tổng Mức độ khẩn cấp
        /// </summary>
        [JsonIgnore]
        public int TotalEmergencyID { get; set; } = 0;
    }

    /// <summary>
    /// Dữ liệu Mức độ khẩn cấp
    /// </summary>
    public class ReportEmergency
    {
        /// <summary>
        /// Thông tin mức độ khẩn cấp
        /// </summary>
        public ValueLabelCheckedModel Emergency { get; set; }

        /// <summary>
        /// Tổng số
        /// </summary>
        public int Total { get; set; } = 0;
    }
}
