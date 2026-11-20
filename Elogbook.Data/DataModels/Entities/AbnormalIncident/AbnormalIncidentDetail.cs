namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AbnormalIncidentDetail")]
    public partial class AbnormalIncidentDetail : EntityBase
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Nội dung xử lý
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

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

        /// <summary>
        /// Trạng thái của bất thường sự cố
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public int? AbnormalIncidentID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AbnormalIncidentID))]
        public virtual AbnormalIncident AbnormalIncident { get; set; }
    }
}
