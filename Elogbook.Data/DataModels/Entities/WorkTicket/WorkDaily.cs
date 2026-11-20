namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("WorkDaily")]
    public partial class WorkDaily : EntityBase
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int? WorkTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public int? WorkLocationID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkLocationID))]
        public virtual ContentPosition WorkLocation { get; set; }

        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public DateTime WorkDay { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Thông tin địa điểm làm việc công tác hằng ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<StaffInformation> StaffInformations { get; set; }

        /// <summary>
        /// Vị trí làm việc
        /// </summary>
        public string WorkLocationText { get; set; }

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
