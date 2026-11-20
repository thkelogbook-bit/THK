using Elogbook.Core.Enums;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Danh mục - Nhận diện mối nguy và biện pháp an toàn
    /// </summary>
    [Table("HazardsAndSafetyMeasures")]
    public partial class HazardsAndSafetyMeasures : EntityBase
    {
        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Nội dung
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// Loại 1: Mối nguy hiểm, 2: Biện pháp an toàn
        /// </summary>
        public int ContentType { get; set; }
    }
}
