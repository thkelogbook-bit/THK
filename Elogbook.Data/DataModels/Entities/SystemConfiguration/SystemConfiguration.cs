using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.SystemConfiguration
{
    /// <summary>
    /// Cấu hình hệ thống
    /// </summary>
    [Table("SystemConfiguration")]
    public class SystemConfiguration : EntityBase
    {

        /// <summary>
        /// Config Key
        /// </summary>
        public string ConfigKey { get; set; }

        /// <summary>
        /// Config Value
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }
}
