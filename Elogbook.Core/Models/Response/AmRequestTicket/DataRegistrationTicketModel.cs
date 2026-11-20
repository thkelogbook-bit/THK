namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// DataRegistrationTicketModel
    /// </summary>
    public class DataRegistrationTicketModel
    {
        /// <summary>
        /// mã số phiếu yêu cầu
        /// </summary>
        public int RequestTicketID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Đơn vị nhận phiếu
        /// </summary>
        public int? ReceiveUnitID { get; set; }

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        public string RegulationNumber { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Địa điểm (hoặc thiết bị) tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }
    }
}
