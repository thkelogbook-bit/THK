using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Quản lý lịch sử Phiếu đăng ký hàng loạt
    /// </summary>
    [Table("BatchWorkRegHistories")]
    public partial class BatchWorkRegHistory : EntityBase
    {
        /// <summary>
        /// Phiếu đăng ký hàng loạt
        /// </summary>
        public int BatchWorkRegID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(BatchWorkRegID))]
        public virtual BatchWorkReg BatchWorkReg { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual RegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkRegNumber { get; set; }

        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Trạng thái (Chưa thực hiện tạo phiếu, Đang tạo phiếu hàng loạt, Hoàn thành tạo phiếu,Có lỗi trong quá trình tạo phiếu)
        /// </summary>
        public int BatchWorkRegStatusID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(BatchWorkRegStatusID))]
        public virtual BatchWorkRegStatus BatchWorkRegStatus { get; set; }

        /// <summary>
        /// ID file đính kèm
        /// </summary>
        public int? AttachmentID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(AttachmentID))]
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// Chú thích
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }
    }
}
