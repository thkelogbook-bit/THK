namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SafetyMeasure")]
    public partial class SafetyMeasure : EntityBase
    {

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int? ReceiveWorkplaceID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(ReceiveWorkplaceID))]
        public virtual ReceiveWorkplace ReceiveWorkplace { get; set; }

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
    }
}
