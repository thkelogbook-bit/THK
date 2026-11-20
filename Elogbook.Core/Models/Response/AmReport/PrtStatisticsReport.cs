namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Số liệu thống kế báo cáo tiến trình phiếu yêu cầu
    /// </summary>
    public class PrtStatisticsReport
    {
        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public ValueLabelCheckedModel Emergency { get; set; }

        /// <summary>
        /// Tổng số phiếu chờ đăng ký
        /// </summary>
        public int TotalWaitForRegist { get; set; } = 0;

        /// <summary>
        /// Tổng số phiếu yêu cầu đã đăng ký
        /// </summary>
        public int TotalRegistered { get; set; } = 0;

        /// <summary>
        /// Tổng số phiếu công tác đã được cấp
        /// </summary>
        public int TotalApproved { get; set; } = 0;

        /// <summary>
        /// Tổng số phiếu công tác đang thực hiện
        /// </summary>
        public int TotalInProgress { get; set; } = 0;

        /// <summary>
        /// Tổng số phiếu công tác đã hoàn thành
        /// </summary>
        public int TotalCompleted { get; set; } = 0;

        /// <summary>
        /// Tổng số
        /// </summary>
        public int TotalAll { get; set; } = 0;

    }
}
