namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin địa điểm công tác
    /// </summary>
    public class PdfWorkPlacementModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// Địa điểm
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký bắt đầu xác nhận bắt đầu làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Tên Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký bắt đầu xác nhận bắt đầu làm việc của người cho phép
        /// </summary>
        public string AllowerSignature { get; set; }
    }
}
