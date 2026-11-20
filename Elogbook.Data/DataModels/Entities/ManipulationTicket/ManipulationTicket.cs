namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ManipulationTicket")]
    public partial class ManipulationTicket : EntityBase
    {
        /// Loại phiếu
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        [Required]
        public int TicketType { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationTicketID { get; set; }
        [ForeignKey(nameof(RegistrationManipulationTicketID))]
        [MapperIgnore]
        public virtual RegistrationManipulationTicket RegistrationManipulationTicket { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Ngày viết phiếu đăng ký
        /// </summary>
        public DateTime CreatedDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Đơn vị cấp phiếu
        /// </summary>
        public int? ApproveUnitID { get; set; }
        [ForeignKey(nameof(ApproveUnitID))]
        [MapperIgnore]
        public virtual Organization ApproveUnit { get; set; }

        /// <summary>
        /// Tên hiển thị đơn vị cấp phiếu
        /// </summary>
        [StringLength(500)]
        public string ApproveUnitView { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        [StringLength(500)]
        public string TicketName { get; set; }

        /// <summary>
        /// Số phiếu thao tác
        /// </summary>
        [StringLength(255)]
        public string TicketNumber { get; set; }

        /// <summary>
        /// 1.1 Tên người viết phiếu
        /// </summary>
        [StringLength(100)]
        public string CreatedUserTicket { get; set; }

        /// <summary>
        /// 1.2 Tên người duyệt phiếu
        /// </summary>
        [StringLength(100)]
        public string ApprovedUserTicket { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public DateTime StartTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public DateTime EndTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// Các sự kiện bất thường trong thao tác
        /// </summary>
        public string CaseAbnormal { get; set; }

        /// <summary>
        /// Thời gian Kết thúc
        /// </summary>
        public DateTime? WorkEndTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        [StringLength(500)]
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Danh sách sơ đồ
        /// </summary>
        [StringLength(500)]
        public string MapAttachmentIDs { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public int TicketStatusValue { get; set; }
        [ForeignKey(nameof(TicketStatusValue))]
        [MapperIgnore]
        public virtual TicketManipulationStatus TicketStatus { get; set; }

        /// <summary>
        /// Người tạo / Login
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
