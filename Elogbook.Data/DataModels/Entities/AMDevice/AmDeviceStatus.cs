using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Tình trạng thiết bị
    /// </summary>
    [Table("AmDeviceStatus")]
    public partial class AmDeviceStatus : EntityBase
    {
        /// <summary>
        /// Tình trạng thiết bị
        /// </summary>
        public string StatusDesc { get; set; }
    }
}
