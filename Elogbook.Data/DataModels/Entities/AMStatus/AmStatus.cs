namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Danh mục - Trạng thái dùng chung
    /// </summary>
    [Table("AmStatus")]
    public partial class AmStatus : EntityBase
    {
        /// <summary>
        /// Loại thiết bị
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StatusName { get; set; }
    }
}
