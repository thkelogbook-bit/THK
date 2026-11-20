using Elogbook.Core.Constants;
using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thông tin đợt Công tác sửa chữa lớn
    /// </summary>
    [Table("MajorRepair")]
    public partial class MajorRepair : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Tên đợt
        /// </summary>
        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        /// <summary>
        /// Trạng thái (Hoàn thành cấp phiếu, Đang cấp phiếu hàng loạt, Chưa thực hiện cấp phiếu)
        /// </summary>
        public int MajorRepairStatusID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(MajorRepairStatusID))]
        public virtual MajorRepairStatus MajorRepairStatus { get; set; }

        /// <summary>
        /// Ngày tạo đợt
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian tạo phiếu công tác
        /// </summary>
        public DateTime PlanCreateTicketTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Kế hoạch bắt đầu thực hiện
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Kế hoạch kết thúc thực hiện
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Tổng số phiếu
        /// </summary>
        public int? TotalTicket { get; set; }

        /// <summary>
        /// Tổng số phiếu thành công
        /// </summary>
        public int? TotalSuccess { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string AttachmentIds { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public int AttachmentID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AttachmentID))]
        public virtual Attachment AttachmentFile { get; set; }

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
