namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin xử lý tồn tại trong ca
    /// </summary>
    public class MobileIssueOfShiftDetailInfo : MobileEntityBaseModel
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
        /// Trạng thái của tồn tại trong ca
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public int? ExistInShiftID { get; set; }
    }
}
