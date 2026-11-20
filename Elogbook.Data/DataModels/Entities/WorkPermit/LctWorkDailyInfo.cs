namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Thông tin công tác hàng ngày/ thi hành lệnh
    /// </summary>
    [Table("LctWorkDailyInfo")]
    public partial class LctWorkDailyInfo : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string CommanderNameElectricRank { get; set; }

        /// <summary>
        /// Người giám sát ATĐ
        /// </summary>
        public string SupervisorName { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string SupervisorUnit { get; set; }

        /// <summary>
        /// Chữ ký người chỉ hủy
        /// </summary>
        public int? CommanderSignature { get; set; }

        /// <summary>
        /// Chữ ký người giám sát
        /// </summary>
        public int? SupervisorSignature { get; set; }

        /// <summary>
        /// Chữ ký người chỉ hủy
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CommanderSignature))]
        public virtual Signature CommanderSignatureInfo { get; set; }

        /// <summary>
        /// Chữ ký người người giám sát
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(SupervisorSignature))]
        public virtual Signature SupervisorSignatureInfo { get; set; }
    }
}
