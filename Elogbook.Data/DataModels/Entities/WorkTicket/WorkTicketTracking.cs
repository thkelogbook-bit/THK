using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("WorkTicketTracking")]
    public partial class WorkTicketTracking : EntityBase
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Người cho phép trước khi thay đổi
        /// </summary>
        public int RequesterUserID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(RequesterUserID))]
        public virtual User RequesterUser { get; set; }

        /// <summary>
        /// Người cho phép hiện tại
        /// </summary>
        public int CurrentAllowerUserID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(CurrentAllowerUserID))]
        public virtual User CurrentAllowerUser { get; set; }

        /// <summary>
        /// Người cho phép mới cần thay đổi
        /// </summary>
        public int? NewAllowerUserID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(NewAllowerUserID))]
        public virtual User NewAllowerUser { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int TrackingStatusID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(TrackingStatusID))]
        public virtual TrackingStatus TrackingStatus { get; set; }

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
