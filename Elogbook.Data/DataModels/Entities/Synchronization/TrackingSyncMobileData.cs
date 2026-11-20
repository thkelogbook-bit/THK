namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TrackingSyncMobileData")]
    public partial class TrackingSyncMobileData : EntityBase
    {
        [Required]
        public string DeviceID { get; set; }

        /// <summary>
        /// Xác định thời gian sync dữ liệu danh mục
        /// </summary>
        [Required]
        public DateTime SyncDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Xác định thời gian sync dữ liệu cho NKVH, TSVH, TTTC, PTT
        /// </summary>
        [Required]
        public DateTime SyncDataModule { get; set; } = DateTime.Now;

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
