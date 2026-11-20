namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ParameterGroup")]
    public partial class ParameterGroup : EntityBase
    {
        /// <summary>
        /// Tên nhóm thông số
        /// </summary>
        [Required]
        [StringLength(255)]
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ cần ghi thông số
        /// </summary>
        public int? CycleID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CycleID))]
        public virtual CycleManagement Cycle { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

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
