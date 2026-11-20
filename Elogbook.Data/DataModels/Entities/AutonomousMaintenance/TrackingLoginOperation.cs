using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Tracking login vào cương vị trên mobile
    /// </summary>
    [Table("TrackingLoginOperation")]
    public partial class TrackingLoginOperation : EntityBase
    {
        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        [Required]
        public int OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }
    }
}
