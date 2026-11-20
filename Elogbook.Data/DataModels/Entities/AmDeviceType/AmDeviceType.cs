using Elogbook.Core.Enums;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Danh mục - Loại thiết bị
    /// </summary>
    [Table("AmDeviceType")]
    public partial class AmDeviceType : EntityBase
    {
        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }

        /// <summary>
        /// PMISType
        /// </summary>
        public string PMISType { get; set; }
    }
}
