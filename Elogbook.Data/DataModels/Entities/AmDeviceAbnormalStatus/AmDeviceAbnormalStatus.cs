namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Danh mục - Trạng thái bất thường thiết bị
    /// </summary>
    [Table("AmDeviceAbnormalStatus")]
    public partial class AmDeviceAbnormalStatus : EntityBase
    {
        /// <summary>
        /// Bất thường thiết bị
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
