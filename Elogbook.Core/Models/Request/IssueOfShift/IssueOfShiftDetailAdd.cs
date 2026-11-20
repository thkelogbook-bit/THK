using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Nội dung xử lý tồn tại trong ca
    /// </summary>
    public class IssueOfShiftDetailAdd
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Mã tồn tại
        /// </summary>
        public int ExistInShiftID { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int CreatedUserID { get; set; }

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
