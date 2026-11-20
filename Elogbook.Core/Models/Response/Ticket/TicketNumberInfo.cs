namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu công tác điện, thủy cơ nhiệt hóa mobile
    /// </summary>
    public class TicketNumberInfo
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Tên người lãnh đạo công việc
        /// </summary>
        public string LeaderName { get; set; }
    }
}
