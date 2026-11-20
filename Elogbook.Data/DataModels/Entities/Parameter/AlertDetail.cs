namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AlertDetails")]
    public partial class AlertDetail : EntityBase
    {
        /// <summary>
        /// Thông số
        /// </summary>
        public int? ParameterID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ParameterID))]
        public virtual Parameter Parameter { get; set; }
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Toán tử
        /// </summary>
        [StringLength(100)]
        public string Alert { get; set; }

        /// <summary>
        /// Toán tử
        /// </summary>
        [StringLength(100)]
        public string Operator { get; set; }

        /// <summary>
        /// Giá trị
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Giá trị
        /// </summary>
        [StringLength(100)]
        public string Color { get; set; }

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
