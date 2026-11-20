namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cho phép
    /// </summary>
    public class PdfAllowedInfoDetailModel
    {
        /// <summary>
        /// Những thiết bị, đường dây, đoạn đường dây đã cắt điện
        /// </summary>
        public string DeviceDisconnected { get; set; }

        /// <summary>
        /// Đã tiếp đất tại các vị trí
        /// </summary>
        public string GroundedAtLocations { get; set; }

        /// <summary>
        /// Đã làm rào chắn và treo biển báo tại
        /// </summary>
        public string BarriersAndWarningPoster { get; set; }

        /// <summary>
        /// Phạm vi được phép làm việc
        /// </summary>
        public string AllowableWorkingRange { get; set; }

        /// <summary>
        /// Cảnh báo, chỉ dẫn cần thiết
        /// </summary>
        public string WarningsAndInstructions { get; set; }
    }
}
