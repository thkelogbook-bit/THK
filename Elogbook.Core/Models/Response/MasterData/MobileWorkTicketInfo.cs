namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class MobileWorkTicketInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel WorkTicketStatus { get; set; }

        /// <summary>
        /// Mã nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Tên người lãnh đạo công việc
        /// </summary>
        public string LeaderName { get; set; }
    }
}
