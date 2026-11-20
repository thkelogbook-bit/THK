using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("ContentPosition")]
    public partial class ContentPosition : EntityBase
    {
        /// <summary>
        /// Mã phiếu đăng ký công tác
        /// </summary>
        public int RegistrationTicketID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string Position { get; set; }

        [MapperIgnore]
        public virtual ICollection<Stakeholders> Stakeholders { get; set; }

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
