namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// AM Trạng thái phiếu yêu cầu
    /// </summary>
    [Table("AmRequestTicketStatus")]
    public partial class AmRequestTicketStatus : EntityBase
    {
        /// <summary>
        /// Tên Trạng thái
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StatusName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }
    }
}
