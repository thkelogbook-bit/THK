using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tồn tại trong ca làm việc
    /// </summary>
    public class AddEditExistInShiftModel
    {
        /// <summary>
        /// Mã tồn tại trong ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Tên phiếu công tác
        /// </summary>
        public string RegistrationTicketName { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Xoá tồn tại trong ca
        /// </summary>
        public bool IsDeleteExistInShift { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }
    }
}
