using Elogbook.Core.Enums;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    [Table("AmSupplier")]
    public partial class AmSupplier : EntityBase
    {
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string SupplierCode { get; set; }

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
