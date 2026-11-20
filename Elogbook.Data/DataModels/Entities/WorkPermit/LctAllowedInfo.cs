namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctAllowedInfo")]
    public partial class LctAllowedInfo : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int? WorkTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Thông tin danh sách cảnh báo
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctAllowedInfoDetail> AllowedInfoDetails { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public DateTime AllowedTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public int? CommanderSignature { get; set; }

        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        public DateTime TimeCommanderSigned { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

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
