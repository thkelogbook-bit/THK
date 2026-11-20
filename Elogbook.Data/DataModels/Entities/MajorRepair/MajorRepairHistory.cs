using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thông tin traking đợt sửa chữa lớn
    /// </summary>
    [Table("MajorRepairHistories")]
    public partial class MajorRepairHistory : EntityBase
    {
        /// <summary>
        /// Sửa chữa lớn
        /// </summary>
        public int? MajorRepairID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(MajorRepairID))]
        public virtual MajorRepair MajorRepair { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public int? WorkTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? MajorRepairStatusID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(MajorRepairStatusID))]
        public virtual MajorRepairStatus MajorRepairStatus { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// ID file đính kèm
        /// </summary>
        public int? AttachmentID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AttachmentID))]
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// Chú thích
        /// </summary>
        public string Comment { get; set; }
    }
}
