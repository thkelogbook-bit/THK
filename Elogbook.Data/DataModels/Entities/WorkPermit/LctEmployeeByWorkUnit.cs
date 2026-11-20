using Elogbook.Data.WorkPermit;

namespace Elogbook.Data.DataModels.Entities.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctEmployeeByWorkUnit")]
    public partial class LctEmployeeByWorkUnit : EntityBase
    {
        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        [StringLength(10)]
        public string LaborRank { get; set; }

        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Chức vụ: trưởng ca, trưởng kíp, ...
        /// </summary>
        [StringLength(500)]
        public string PositionName { get; set; }

        /// <summary>
        /// Vai trò của người tham gia (lãnh đạo, chỉ huy, giám sát,...)
        /// </summary>
        public int? Roles { get; set; }

        /// <summary>
        /// cấp bật ATĐ
        /// </summary>
        [StringLength(50)]
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        [StringLength(255)]
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }
    }
}
