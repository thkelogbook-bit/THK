using System;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Trình tự hạng mục thao tác
    /// </summary>
    public class SequenceOperationSyncModel
    {
        /// <summary>
        /// Mới hay đã tạo trước đó
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã phiếu thao tác được tạo hoặc mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool IsCreated { get; set; }

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
        public int Step { get; set; }

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
        public UserInfoInTicketModel LeaderUser { get; set; }

        /// <summary>
        /// 1.2 Người nhận lệnh
        /// </summary>
        public UserInfoInTicketModel WorkerUser { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Mã số step cha
        /// </summary>
        public int? ParentStep { get; set; }
    }
}
