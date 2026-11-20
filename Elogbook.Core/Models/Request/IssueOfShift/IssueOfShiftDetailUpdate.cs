using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cập nhật xử lý tồn tại trong ca
    /// </summary>
    public class IssueOfShiftDetailUpdate
    {
        /// <summary>
        /// Mã thông tin xử lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã tồn tại
        /// </summary>
        public int ExistInShiftID { get; set; }

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
