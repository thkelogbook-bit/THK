using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Các trạng thái của report
    /// </summary>
    [Table("AmReportStatus")]
    public partial class AmReportStatus : EntityBase
    {
        /// <summary>
        /// Tên Trạng thái
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StatusName { get; set; }

        /// <summary>
        /// Loại trạng thái
        /// </summary>
        public int StatusType { get; set; }
    }
}
