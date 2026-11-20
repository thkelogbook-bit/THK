using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cập nhật xử lý bất thường sự cố
    /// </summary>
    public class AbnormalIncidentDetailUpdate
    {
        /// <summary>
        /// Mã thông tin xử lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã bất thường sự cố
        /// </summary>
        public int AbnormalIncidentID { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int UpdatedUserID { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }

        /// <summary>
        /// Kết thúc xử lý
        /// </summary>
        public bool IsComplete { get; set; }
    }
}
