namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WorkTicket")]
    public partial class WorkTicket : EntityBase
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        [Required]
        [StringLength(100)]
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual RegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu (cấp phân xưởng)
        /// </summary>
        public int? ApprovedUnitID { get; set; }
        [ForeignKey(nameof(ApprovedUnitID))]
        [MapperIgnore]
        public virtual Organization ApprovedUnit { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Loại phiếu (Kế hoạch, Ngoài kế hoạch,Xử lý sự cố, Phiếu công tác thuộc Đợt công tác SCL )
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái phiếu công tác (Đã cấp phiếu công tác, Đã cho phép, Đang thực hiện, Chờ xác nhận hoàn thành, ...)
        /// </summary>
        public int WorkTicketStatus { get; set; }
        [ForeignKey(nameof(WorkTicketStatus))]
        [MapperIgnore]
        public virtual TicketStatus TicketStatus { get; set; }

        /// <summary>
        /// Nhân viên đơn vị công tác
        /// </summary>
        public int? UnitNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Phạm vi làm việc
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thông tin làm việc của nhân viên
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkerInformation> WorkerInformations { get; set; }

        /// <summary>
        /// Thông tin địa điểm làm việc công tác hằng ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkPlacement> WorkPlacements { get; set; }

        /// <summary>
        /// Thông tin ban quản lý (người cấp phiếu, người cho phép, người chỉ huy, người giám sát, người cảnh giới, người lãnh đạo)
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LeadershipUser> LeadershipUsers { get; set; }

        /// <summary>
        /// FIle đính kèm
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

        /// <summary>
        /// Hình ảnh quá trình công tác
        /// </summary>
        public string WorkPictures { get; set; }

        /// <summary>
        /// Kết quả công việc
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Cảnh báo nguy hiểm và biện pháp an toàn
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WarnAndSafe> WarnAndSafes { get; set; }

        /// <summary>
        /// Các loại phiếu TCNH (Thủy/Cơ/Nhiệt/Hóa/Xây dựng)
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkTicketFeatureLink> WorkTicketFeatureLinks { get; set; }

        /// <summary>
        /// Công tác hằng ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkDaily> WorkDailys { get; set; }

        /// <summary>
        /// Trạng thái phiếu công tác TCNH (Thủy/Cơ/Nhiệt/Hóa/Xây dựng)
        /// </summary>
        public int? WorkTicketFeatureID { get; set; }
        [ForeignKey(nameof(WorkTicketFeatureID))]
        [MapperIgnore]
        public virtual WorkTicketFeatures WorkTicketFeature { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// File phiếu đã cấp
        /// </summary>
        public int? TicketPdfID { get; set; }

        /// <summary>
        /// Trạng thái thay đổi người cho phép
        /// </summary>
        public int? TrackingStatusID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(TrackingStatusID))]
        public virtual TrackingStatus TrackingStatus { get; set; }

        /// <summary>
        /// Thay đổi người cho phép
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkTicketTracking> WorkTicketTrackings { get; set; }

        /// <summary>
        /// Đợt sửa chữa lớn
        /// </summary>
        public int? MajorRepairID { get; set; }
        [ForeignKey(nameof(MajorRepairID))]
        [MapperIgnore]
        public virtual MajorRepair MajorRepair { get; set; }

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

        /// <summary>
        /// Mã phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Lý do từ chối (không cho phép thực hiện hoặc hủy phiếu)
        /// </summary>
        public string ReasonReject { get; set; }

        /// <summary>
        /// Đánh dấu các mốc thời gian đã gửi thông báo cho người dùng khi đến hạn cần thực hiện
        /// </summary>
        public string BeforeDueHistory { get; set; }

        /// <summary>
        /// Đánh dấu các mốc thời gian đã gửi thông báo cho người dùng khi đến hạn cần thực hiện trong ngày
        /// </summary>
        public string DueTodayHistory { get; set; }

        /// <summary>
        /// Đánh dấu các mốc thời gian đã gửi thông báo cho người dùng khi quá hạn cần thực hiện
        /// </summary>
        public string OverdueHistory { get; set; }
    }
}
