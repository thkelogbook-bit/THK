namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("KnowledgeBase")]
    public partial class KnowledgeBase : EntityBase
    {
        /// <summary>
        /// Chủ đề
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Title { get; set; }

        /// <summary>
        /// Mô tả vắn tắt nội dung tài liệu
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Mã tài liệu
        /// </summary>
        [Required]
        [StringLength(100)]
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
        /// Phiên bản tài liệu
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Giá trị module xác định tài liệu thuộc module nào
        /// </summary>
        [StringLength(50)]
        public string ModuleValue { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
