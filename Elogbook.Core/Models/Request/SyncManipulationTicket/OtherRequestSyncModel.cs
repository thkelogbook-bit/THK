namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Thông tin các yêu cầu liên quan phiếu thao tác
    /// </summary>
    public class OtherRequestSyncModel
    {
        /// <summary>
        /// Mã thông tin yêu cầu
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
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 1/Điều kiện cần có thể thực hiện, 2/Lưu ý
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Mới hay đã tạo trước đó
        /// </summary>
        public bool IsNew { get; set; }
    }
}
