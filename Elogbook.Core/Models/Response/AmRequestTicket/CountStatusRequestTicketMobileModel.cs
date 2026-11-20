namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// CountStatusRequestTicketMobileModel
    /// </summary>
    public class CountStatusRequestTicketMobileModel
    {
        /// <summary>
        /// số lượng tab đang xử lý
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        /// số lượng tab đã duyệt
        /// </summary>
        public int Approved { get; set; }

        /// <summary>
        /// số lượng tab không duyệt
        /// </summary>
        public int Reject { get; set; }

        /// <summary>
        /// số lượng tab đã đăng ký
        /// </summary>
        public int Registered { get; set; }
    }
}
