namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OtherRequirements")]
    public class OtherRequirements : EntityBase
    {
        /// <summary>
        /// Vị trí
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Biện pháp an toàn / Tiếp đất di động
        /// </summary>
        public string SafetyMeasures { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual RegistrationTicket RegistrationTicket { get; set; }

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
