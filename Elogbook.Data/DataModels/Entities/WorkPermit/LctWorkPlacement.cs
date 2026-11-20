namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkPlacement")]
    public partial class LctWorkPlacement : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Địa điểm công tác
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Điều kiện an toàn
        /// </summary>
        public string SafeCondition { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        [StringLength(100)]
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public int? CommanderSignature { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        [StringLength(100)]
        public string AcceptorName { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public int? AcceptorSignature { get; set; }

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
        /// Chữ ký người chỉ hủy
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CommanderSignature))]
        public virtual Signature CommanderSignatureInfo { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(AcceptorSignature))]
        public virtual Signature AcceptorSignatureInfo { get; set; }
    }
}
