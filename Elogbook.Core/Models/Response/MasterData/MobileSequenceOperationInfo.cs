using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Table lưu dữ liệu trình tự hạng mục thao tác
    /// </summary>
    public class MobileSequenceOperationInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// mã phiếu thao tác được cấp
        /// </summary>
        public int? TicketID { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationID { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký thao tác mẫu
        /// </summary>
        public int? ManipulationTemplateTicketID { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool? IsCreated { get; set; }

        /// <summary>
        /// Mục
        /// </summary>
        public string TableOfContents { get; set; }

        /// <summary>
        /// Địa điểm
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Bước
        /// </summary>
        public int? Step { get; set; }

        /// <summary>
        /// Trình tự thao tác (nội dung)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 1.2 Người ra lệnh
        /// </summary>
        public int? LeaderID { get; set; }

        /// <summary>
        /// 1.2 Người nhận lệnh
        /// </summary>
        public int? WorkerID { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsDone { get; set; }

        /// <summary>
        /// Mã số step cha
        /// </summary>
        public int? ParentStep { get; set; }
    }
}
