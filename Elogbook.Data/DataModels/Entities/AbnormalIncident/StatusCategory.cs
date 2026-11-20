namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StatusCategory")]
    public partial class StatusCategory : EntityBase
    {
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        [Required]
        [StringLength(255)]
        public string StatusName { get; set; }

        /// <summary>
        /// Loại dữ liệu trạng thái
        /// </summary>
        public int DataTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(DataTypeID))]
        public virtual SystemData DataType { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int? Position { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
