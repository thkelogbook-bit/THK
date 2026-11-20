namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổng các hoạt động AM
    /// </summary>
    public class DashboardSummaryGroupModel
    {
        /// <summary>
        /// Tổng các hoạt động AM
        /// </summary>
        public AmSummaryInfo AmSummaryInfo { get; set; }

        /// <summary>
        /// Số bất thường thiết bị
        /// </summary>
        public DeviceAbnormalSummaryInfo DeviceAbnormalSummaryInfo { get; set; }

        /// <summary>
        /// Số phiếu yêu cầu đã tạo
        /// </summary>
        public RequestTicketSummaryInfo RequestTicketSummaryInfo { get; set; }

        /// <summary>
        /// Số nhân sự tham gia
        /// </summary>
        public NumberOfParticipantsInfo NumberOfParticipantsInfo { get; set; }
    }

    /// <summary>
    /// Tổng các hoạt động AM
    /// </summary>
    public class AmSummaryInfo
    {
        /// <summary>
        /// Tổng chưa thực hiện
        /// </summary>
        public int TotalWaiting { get; set; } = 0;

        /// <summary>
        /// Tổng không thực hiện
        /// </summary>
        public int TotalDoNotWork { get; set; } = 0;

        /// <summary>
        /// Tổng đang thực hiện
        /// </summary>
        public int TotalOnGoing { get; set; } = 0;

        /// <summary>
        /// Tổng hoàn thành
        /// </summary>
        public int TotalCompleted { get; set; } = 0;

        /// <summary>
        /// Tổng hoạt động
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Số bất thường thiết bị
    /// </summary>
    public class DeviceAbnormalSummaryInfo
    {
        /// <summary>
        /// Tổng Phiếu yêu cầu chưa tạo
        /// </summary>
        public int TotalUnCreated { get; set; } = 0;

        /// <summary>
        /// Tổng Phiếu yêu cầu đã tạo
        /// </summary>
        public int TotalCreated { get; set; } = 0;

        /// <summary>
        /// Tổng
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Số phiếu yêu cầu đã tạo
    /// </summary>
    public class RequestTicketSummaryInfo
    {
        /// <summary>
        /// Tổng chưa gửi phiếu
        /// </summary>
        public int TotalUnSentPYC { get; set; } = 0;

        /// <summary>
        /// Tổng đang xử lý
        /// </summary>
        public int TotalInProgessPYC { get; set; } = 0;

        /// <summary>
        /// Tổng đã được duyệt
        /// </summary>
        public int TotalApprovePYC { get; set; } = 0;

        /// <summary>
        /// Tổng dừng xử lý
        /// </summary>
        public int TotalStopPYC { get; set; } = 0;

        /// <summary>
        /// Tổng
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Số nhân sự tham gia
    /// </summary>
    public class NumberOfParticipantsInfo
    {
        /// <summary>
        /// Tổng số nhân viên
        /// </summary>
        public int TotalUser { get; set; } = 0;
    }
}
