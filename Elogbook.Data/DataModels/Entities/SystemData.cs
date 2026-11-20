namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SystemData")]
    public partial class SystemData : EntityBase
    {
        /// <summary>
        /// Loại dữ liệu
        /// </summary>
        [Required]
        public int DataType { get; set; }

        /// <summary>
        /// Tên loại dữ liệu
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Yêu cầu đồng bộ dữ liệu trên Mobile
        /// </summary>
        [Required]
        public bool IsSyncDataMobile { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        [Required]
        public int Position { get; set; }
    }
}
