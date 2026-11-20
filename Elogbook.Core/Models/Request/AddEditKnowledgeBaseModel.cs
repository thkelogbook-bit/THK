using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tài liệu
    /// </summary>
    public class AddEditKnowledgeBaseModel
    {
        /// <summary>
        /// Mã tài liệu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Chủ đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Mô tả vắn tắt nội dung tài liệu
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Mã tài liệu
        /// </summary>
        public string ProcessDocumentCode { get; set; }

        /// <summary>
        /// Tài liệu đính kèm
        /// </summary>
        public int? AttachmentID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Giá trị module xác định tài liệu thuộc module nào
        /// </summary>
        public string ModuleValue { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
