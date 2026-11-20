namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số thủ thục cho phép công tác
    /// </summary>
    public class PdfAllowedInfoModel
    {
        /// <summary>
        /// Thủ tục cho phép công tác
        /// </summary>
        public string Title { get; set; }

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

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chức vụ người cho phép
        /// </summary>
        public string AllowerPosition { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public string AllowerSignature { get; set; }
    }
}
