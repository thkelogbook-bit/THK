using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Trạng thái tạo Phiếu đăng ký hàng loạt
    /// </summary>
    [Table("BatchWorkRegStatus")]
    public partial class BatchWorkRegStatus : EntityBase
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; }
    }
}
