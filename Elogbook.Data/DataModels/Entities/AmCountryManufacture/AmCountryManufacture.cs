using Elogbook.Core.Enums;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Nước sản xuất
    /// </summary>
    [Table("AmCountryManufacture")]
    public partial class AmCountryManufacture : EntityBase
    {
        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Mã code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
