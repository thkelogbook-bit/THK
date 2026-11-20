using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình chung
    /// </summary>
    [Table("ConfigurationWarnings")]
    public partial class ConfigurationWarning : EntityBase
    {
        /// <summary>
        /// Khoảng dữ liệu phân tích (đơn vị ngày)
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; }
    }
}
