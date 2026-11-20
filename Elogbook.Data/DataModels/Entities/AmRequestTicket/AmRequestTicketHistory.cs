namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// AM lịch sử trạng thái phiếu yêu cầu
    /// </summary>
    [Table("AmRequestTicketHistory")]
    public class AmRequestTicketHistory : EntityBase
    {
        /// <summary>
        /// Mã phiếu yêu cầu
        /// </summary>
        public int RequestTicketId { get; set; }

        /// <summary>
        /// Hành động
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Miêu tả
        /// </summary>
        public string Comment { get; set; }

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
