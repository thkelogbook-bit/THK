using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("TrackingStatus")]
    public partial class TrackingStatus : EntityBase
    {
        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public string Status { get; set; }
    }
}
