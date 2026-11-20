namespace Elogbook.Data
{
    using Elogbook.Core.Enums;
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Danh mục - Nguyên nhân không thực hiện (Dùng cho Không thực hiện Bảo trì tự quản và Dừng 1 thực hiện AM cụ thể )
    /// </summary>
    [Table("AmReasonForStop")]
    public partial class AmReasonForStop : EntityBase
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
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

        /// <summary>
        /// Loại 1: Không thực hiện bảo trì, 2: Dừng công việc
        /// </summary>
        public int ReasonType { get; set; }
    }
}
