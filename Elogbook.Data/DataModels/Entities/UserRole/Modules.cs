namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Modules")]
    public partial class Modules : EntityBase
    {
        /// <summary>
        /// Mã định danh module
        /// </summary>
        [StringLength(50)]
        [Required]
        public string ModuleValue { get; set; }

        /// <summary>
        /// Tên module
        /// </summary>
        [StringLength(255)]
        [Required]
        public string ModuleName { get; set; }

        /// <summary>
        /// Module thuộc Admin Portal hay Mobile App
        /// </summary>
        [StringLength(50)]
        [Required]
        public string SystemType { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
