using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Công tác đang thực hiện
    /// </summary>
    public class AddEditActiveWorkModel
    {
        /// <summary>
        /// Mã công tác đang thực hiện
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Giờ, phút bắt đầu
        /// </summary>
        public string StartHour { get; set; }

        /// <summary>
        /// Giờ, phút kết thúc
        /// </summary>
        public string EndHour { get; set; }

        /// <summary>
        /// Nội dung công tác đang thực hiện
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }
    }
}
