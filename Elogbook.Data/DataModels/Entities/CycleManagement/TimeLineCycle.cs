namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TimeLineCycle")]
    public partial class TimeLineCycle : EntityBase
    {
        /// <summary>
        /// Mốc thời gian
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        [Required]
        public int? CycleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CycleID))]
        public virtual CycleManagement Cycle { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

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
