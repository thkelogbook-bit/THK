namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WorkPlacement")]
    public partial class WorkPlacement : EntityBase
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Địa điểm công tác
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        [StringLength(100)]
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký xác nhận bắt đầu làm việc của người chỉ huy trực tiếp
        /// </summary>
        public int? CommanderSignature { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        [StringLength(100)]
        public string AcceptorName { get; set; }

        /// <summary>
        /// Chữ ký xác nhận bắt đầu làm việc của người cho phép
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
    }
}
