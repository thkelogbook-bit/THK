namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctReceiveWorkplace")]
    public partial class LctReceiveWorkplace : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int? WorkTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Danh sách các biện pháp
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctSafetyMeasure> SafetyMeasures { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        [StringLength(255)]
        public string WorkUnitName { get; set; }

        /// <summary>
        /// Đơn vị công tác (cấp tổ)
        /// </summary>
        public int? WorkUnitID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkUnitID))]
        public virtual Organization WorkUnit { get; set; }

        /// <summary>
        /// Số lượng người
        /// </summary>
        [StringLength(255)]
        public string AmountPersons { get; set; }

        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        public DateTime StartWorkingDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Chữ ký người lãnh đạo
        /// </summary>
        public int? LeaderSignature { get; set; }

        /// <summary>
        /// Chữ ký người chỉ hủy
        /// </summary>
        public int? CommanderSignature { get; set; }

        /// <summary>
        /// Chữ ký người giám sát công việc
        /// </summary>
        public int? SupervisorSignature { get; set; }

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
