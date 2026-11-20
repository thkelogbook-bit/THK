namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctAllowedInfoDetail")]
    public partial class LctAllowedInfoDetail : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int AllowedInforID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AllowedInforID))]
        public virtual LctAllowedInfo AllowedInfo { get; set; }

        /// <summary>
        /// Nội dung cho phép
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Đã được đánh dấu
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
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
    }
}
