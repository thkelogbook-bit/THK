namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Từ chối cấp phiếu 
    /// </summary>
    public class RejectRequirementModel: EntityBaseModel
    {
        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public ValueLabelModel WorkRegistrationTicket { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Lý do từ chối
        /// </summary>
        public string ReasonOfReject { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public string AttachFile { get; set; }
    }
}
