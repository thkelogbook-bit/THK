namespace Elogbook.Data
{
    using Elogbook.Core.Enums;
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Danh mục - Địa điểm/Khu vực
    /// </summary>
    [Table("AmLocation")]
    public partial class AmLocation : EntityBase
    {
        /// <summary>
        /// Công ty
        /// </summary>
        [Required]
        public int OrganizationID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OrganizationID))]
        public virtual Organization Organization { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Khu vực cha
        /// </summary>
        public string SiteParent { get; set; } = null;

        /// <summary>
        /// Tên địa điểm
        /// </summary>
        public string SiteDesc { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string SiteCode { get; set; }

        /// <summary>
        /// Mã PMIS
        /// </summary>
        public string PMISId { get; set; }
    }
}
