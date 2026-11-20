namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkerInformation")]
    public partial class LctWorkerInformation : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Tên công nhân
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        public DateTime CheckInTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public int? CheckInSignature { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public DateTime CheckOutTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public int? CheckOutSignature { get; set; }

        /// <summary>
        /// Tài khoản người dùng nội bộ/ngoài hệ thống
        /// </summary>
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Xác định người dùng nội bộ/ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }

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
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRankStr { get; set; }

    }
}
