namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RegistrationManipulationTicket")]
    public partial class RegistrationManipulationTicket : EntityBase
    {
        /// Loại phiếu
        /// 1/kế hoạch 2/Đột xuất
        /// </summary>
        [Required]
        public int TicketTypeValue { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

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
        [StringLength(255)]
        public string ApproveUnitView { get; set; }

        /// <summary>
        /// 1. Tên phiếu thao tác
        /// </summary>
        [StringLength(500)]
        public string TicketName { get; set; }

        /// <summary>
        /// 1.1 Chức vụ người tạo phiếu
        /// </summary>
        [StringLength(255)]
        public string CreatedUserPositionName { get; set; }

        /// <summary>
        /// 1.2 Người phê duyệt
        /// </summary>
        public int ApprovedUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ApprovedUserID))]
        public virtual User ApprovedUser { get; set; }

        /// <summary>
        /// 1.2 Chức vụ người phê duyệt
        /// </summary>
        [StringLength(255)]
        public string ApprovedUserPositionName { get; set; }

        /// <summary>
        /// 2. Mục đích thao tác
        /// </summary>
        [Required]
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
        /// Danh sách tài liệu đính kèm
        /// </summary>
        [StringLength(500)]
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Trạng thái phiếu thao tác
        /// </summary>
        public int TicketStatusValue { get; set; }
        [ForeignKey(nameof(TicketStatusValue))]
        [MapperIgnore]
        public virtual TicketManipulationStatus TicketStatus { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Thông tin trạng thái phiếu thao tác
        /// </summary>
        public virtual ICollection<TicketStatusHistory> TicketStatusHistories { get; set; }

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

