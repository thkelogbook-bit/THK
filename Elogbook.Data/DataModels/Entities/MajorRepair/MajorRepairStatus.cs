using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Trạng thái Công tác sửa chữa lớn
    /// </summary>
    [Table("MajorRepairStatus")]
    public partial class MajorRepairStatus : EntityBase
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; }
    }
}
