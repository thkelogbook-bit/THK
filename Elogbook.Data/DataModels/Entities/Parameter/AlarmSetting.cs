namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AlarmSettings")]
    public partial class AlarmSetting : EntityBase
    {
        [Required]
        [StringLength(100)]
        public string SettingName { get; set; }

        [Required]
        public int SettingType { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        [StringLength(100)]
        public string ColorCode { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
