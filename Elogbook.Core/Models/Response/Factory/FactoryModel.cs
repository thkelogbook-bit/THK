namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhà máy
    /// </summary>
    public class FactoryModel : EntityBaseModel
    {
        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public ValueLabelModel Company { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Cấu hình sử dụng nhóm thông số trong thông số vận hành
        /// </summary>
        public bool IsUseOperationParameterGroup { get; set; }

        /// <summary>
        /// Tên viết tắt của nhà máy
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Cho phép sử dụng tên viết tắt của nhà máy trong số phiếu
        /// </summary>
        public bool IsUseShortNameToTicket { get; set; }

        /// <summary>
        /// Format phiếu thao tác
        /// </summary>
        public string PTTFormat { get; set; }

        /// <summary>
        /// Format phiếu công tác điện
        /// </summary>
        public string PCTDFormat { get; set; }

        /// <summary>
        /// Format phiếu công tác thủy cơ nhiệt hóa
        /// </summary>
        public string PCTTCNHFormat { get; set; }

        /// <summary>
        /// Email chung của nhà máy
        /// </summary>
        public string Email { get; set; }
    }
}
