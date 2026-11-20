namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin xử lý tồn tại trong ca
    /// </summary>
    public class MobileAbnormalIncidentDetailInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Nội dung xử lý
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Trạng thái của bất thường sự cố
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// Mã bất thường sự cố
        /// </summary>
        public int? AbnormalIncidentID { get; set; }
    }
}
