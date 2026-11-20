namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table lưu dữ liệu trình tự hạng mục thao tác
    /// </summary>
    [Table("SequenceOperation")]
    public partial class SequenceOperation : EntityBase
    {
        /// <summary>
        /// mã phiếu thao tác được cấp
        /// </summary>
        public int? TicketID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Mã phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationID { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký thao tác mẫu
        /// </summary>
        public int? ManipulationTemplateTicketID { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool IsCreated { get; set; }

        /// <summary>
        /// Mục
        /// </summary>
        [StringLength(255)]
        public string TableOfContents { get; set; }

        /// <summary>
        /// Địa điểm
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Bước
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Trình tự thao tác (nội dung)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 1.2 Người ra lệnh
        /// </summary>
        public int? LeaderID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LeaderID))]
        public virtual SequenceOperationParticipant LeaderUser { get; set; }

        /// <summary>
        /// 1.2 Người nhận lệnh
        /// </summary>
        public int? WorkerID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkerID))]
        public virtual SequenceOperationParticipant WorkerUser { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Mã số step cha
        /// </summary>
        public int? ParentStep { get; set; }

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
