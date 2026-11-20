namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IdentificationAndSafetyMeasures")]
    public class IdentificationAndSafetyMeasures : EntityBase
    {
        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual RegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Nhận diện mối nguy
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// biện pháp an toàn
        /// </summary>
        public string SafetyMeasures { get; set; }

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
