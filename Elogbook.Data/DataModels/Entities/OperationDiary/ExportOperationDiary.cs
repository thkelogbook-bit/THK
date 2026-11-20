using Elogbook.Core.Enums;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("ExportOperationDiary")]
    public partial class ExportOperationDiary : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }

        /// <summary>
        /// Ngày tạo nhật ký từ ngày
        /// </summary>
        [Required]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Ngày tạo nhật ký đến ngày
        /// </summary>
        [Required]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationShiftID))]
        public virtual OperationShift OperationShift { get; set; }

        /// <summary>
        /// Trạng thái export (lưu dạng string trong DB)
        /// </summary>
        [Column("Status")]
        public string StatusString
        {
            get => Status.ToString();
            set => Status = Enum.TryParse(value, out ExportStatus result) ? result : ExportStatus.Waiting;
        }

        [NotMapped]
        public ExportStatus Status { get; set; } = ExportStatus.Waiting;

        /// <summary>
        /// File đính kèm
        /// </summary>
        public int? AttachmentID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AttachmentID))]
        public virtual Attachment AttachmentFile { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
