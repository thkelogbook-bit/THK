using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Quản lý Phiếu đăng ký hàng loạt
    /// </summary>
    [Table("BatchWorkRegs")]
    public partial class BatchWorkReg : EntityBase
    {
        /// <summary>
        /// Phân loại màn hình (1:Điện, 2:Thủy cơ nhiệt hóa)
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Trạng thái (Chưa thực hiện tạo phiếu, Đang tạo phiếu hàng loạt, Hoàn thành tạo phiếu,Có lỗi trong quá trình tạo phiếu)
        /// </summary>
        public int BatchWorkRegStatusID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(BatchWorkRegStatusID))]
        public virtual BatchWorkRegStatus BatchWorkRegStatus { get; set; }

        /// <summary>
        /// Ngày tạo đợt
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian tạo phiếu
        /// </summary>
        public DateTime PlanCreateTicketTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Tổng số phiếu
        /// </summary>
        public int? TotalTicket { get; set; }

        /// <summary>
        /// Tổng số phiếu thành công
        /// </summary>
        public int? TotalSuccess { get; set; }

        /// <summary>
        /// Lịch sử cập nhật file data(nếu có)
        /// </summary>
        public string AttachmentIds { get; set; }

        /// <summary>
        /// File data
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
