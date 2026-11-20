namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Table lưu dữ liệu 1/Điều kiện cần có thể thực hiện, 2/Lưu ý
    /// </summary>
    public class MobileOtherRequestInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// mã phiếu thao tác được cấp
        /// </summary>
        public int? TicketID { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationID { get; set; }

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

    }
}
