namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UsedTimes")]
    public partial class UsedTimes : EntityBase
    {
        /// <summary>
        /// Thời gian bắt đầu sử dụng ca vận hành
        /// </summary>
        public DateTime? ActiveTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc sử dụng ca vận hành
        /// </summary>
        public DateTime? InactiveTime { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int? OperationShiftID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationShiftID))]
        public virtual OperationShift OperationShift { get; set; }

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
