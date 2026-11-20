namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Attachment")]
    public partial class Attachment : EntityBase
    {
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Đường dẫn vật lý lưu tập tin
        /// </summary>
        [StringLength(500)]
        public string FilePath { get; set; }

        /// <summary>
        /// Đường dẫn vật lý lưu tập tin thu nhỏ
        /// </summary>
        [StringLength(500)]
        public string PathThumbnail { get; set; }

        /// <summary>
        /// Tên tập tin
        /// </summary>
        [StringLength(500)]
        public string FileName { get; set; }

        /// <summary>
        /// Kiểu tập tin
        /// </summary>
        [StringLength(255)]
        public string FileType { get; set; }

        /// <summary>
        /// Nội dung mô tả về tập tin
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Vị trí hiển thị tập tin
        /// </summary>
        [StringLength(100)]
        public string ScreenName { get; set; }

        /// <summary>
        /// Source ID trong Open Notebook (dạng "source:xxx").
        /// Được gán khi file được upload lên Open Notebook.
        /// Dùng để map reference trong câu trả lời AI với tên file gốc.
        /// </summary>
        [StringLength(100)]
        public string OpenNotebookSourceId { get; set; }

        /// <summary>
        /// Giá trị module liên kết với tập tin đính kèm
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
