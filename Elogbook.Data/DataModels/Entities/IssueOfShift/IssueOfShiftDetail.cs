namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IssueOfShiftDetail")]
    public partial class IssueOfShiftDetail : EntityBase
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
        [StringLength(500)]
        public string Attachments { get; set; }

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

        /// <summary>
        /// Trạng thái của tồn tại trong ca
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public int? ExistInShiftID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(ExistInShiftID))]
        public virtual ExistInShift ExistInShift { get; set; }
    }
}
