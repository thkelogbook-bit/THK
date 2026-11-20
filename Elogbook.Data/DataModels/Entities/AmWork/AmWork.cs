namespace Elogbook.Data
{
    using Elogbook.Core.Enums;
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Danh mục - Công việc
    /// </summary>
    [Table("AmWork")]
    public partial class AmWork : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        [Required]
        public int AmWorkTypeID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AmWorkTypeID))]
        public virtual AmWorkType AmWorkType { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string WorkDesc { get; set; }
    }
}
