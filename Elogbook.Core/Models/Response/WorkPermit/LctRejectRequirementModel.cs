namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Từ chối cấp lệnh 
    /// </summary>
    public class LctRejectRequirementModel: EntityBaseModel
    {
        /// <summary>
        /// lệnh đăng ký công tác
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
