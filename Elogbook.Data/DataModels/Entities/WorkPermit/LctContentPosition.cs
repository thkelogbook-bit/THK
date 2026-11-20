namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctContentPosition")]
    public partial class LctContentPosition : EntityBase
    {
        /// <summary>
        /// Mã lệnh đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string Position { get; set; }

        [MapperIgnore]
        public virtual ICollection<LctStakeholders> Stakeholders { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual LctRegistrationTicket RegistrationTicket { get; set; }

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
