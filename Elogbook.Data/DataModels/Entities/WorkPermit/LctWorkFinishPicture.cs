namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkFinishPicture")]
    public partial class LctWorkFinishPicture : EntityBase
    {
        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        public int? WorkFinishID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkFinishID))]
        public virtual LctWorkFinish WorkFinish { get; set; }

        /// <summary>
        /// Danh sách file hình ảnh
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

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
