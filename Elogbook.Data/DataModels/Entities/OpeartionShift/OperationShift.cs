namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationShift")]
    public partial class OperationShift : EntityBase
    {
        /// <summary>
        /// Tên ca
        /// </summary>
        [Required]
        [StringLength(255)]
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        [Required]
        public int Position { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        [Required]
        public double StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        [Required]
        public double EndTime { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Các mốc thời gian sử dụng ca vận hành
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<UsedTimes> UsedTimes { get; set; }

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
